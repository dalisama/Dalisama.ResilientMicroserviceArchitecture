using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dalisama.ResilientMicroserviceArchitecture.Common
{


    public abstract class ConsumeRabbitMQHostedServiceBase<T, V> : BackgroundService where T : ServiceStatus where V : Domain
    {
        protected readonly ILogger _logger;
        private IConnection _connection;
        private IModel _channel;
        private T _serviceStatus;
        private EventingBasicConsumer _consumer;
        private CancellationTokenSource _cancellationToken = new CancellationTokenSource();
        private RabbitMqExchangeConfiguration _rabbitMqExchangeConfiguration;
        private RabbitMqQueueConfiguration _rabbitMqQueueConfiguration;
        private V _domain;

        public ConsumeRabbitMQHostedServiceBase(ILogger<ConsumeRabbitMQHostedServiceBase<T, V>> logger, T serviceStatus, V domain, IOptions<RabbitMqExchangeConfiguration> rabbitMqExchangeConfiguration, IOptions<RabbitMqQueueConfiguration> rabbitMqQueueConfiguration)
        {

            _logger = logger;
            _rabbitMqExchangeConfiguration = rabbitMqExchangeConfiguration.Value;
            _rabbitMqQueueConfiguration = rabbitMqQueueConfiguration.Value;
            InitRabbitMQ();
            _serviceStatus = serviceStatus;
            _domain = domain;
            _serviceStatus.ServiceHealthDegaded += OnServiceHealthDegarded;
            _serviceStatus.ServiceHealthUp += OnServiceHealthUp;

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory
            {
                HostName = _rabbitMqExchangeConfiguration.Hostname,
                Password = _rabbitMqExchangeConfiguration.Password,
                VirtualHost = _rabbitMqExchangeConfiguration.VirtualHost,
                UserName = _rabbitMqExchangeConfiguration.UserName,
                Port = _rabbitMqExchangeConfiguration.Port

            };

            // create connection  
            _connection = factory.CreateConnection();

            // create channel  
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(_rabbitMqExchangeConfiguration.ExchangeName, _rabbitMqExchangeConfiguration.ExchangeType);
            _channel.QueueDeclare(_rabbitMqQueueConfiguration.QueueName, _rabbitMqQueueConfiguration.Durable, _rabbitMqQueueConfiguration.Exclusive, _rabbitMqQueueConfiguration.Autodelete, null);
            _channel.QueueBind(_rabbitMqQueueConfiguration.QueueName, _rabbitMqExchangeConfiguration.ExchangeName, _rabbitMqQueueConfiguration.Key, null);
            _channel.BasicQos(0, 1, true);


        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            _consumer = new EventingBasicConsumer(_channel);

            _consumer.Received += (ch, ea) =>
            {
               


                    var content = (Message)ea.Body;
                    bool result;
                    if (!content.MetaData.ContainsKey("Sequence id")) content.MetaData["Sequence id"] = Guid.NewGuid().ToString();
                    if (!content.MetaData.ContainsKey("Transaction id")) content.MetaData["Transaction id"] = Guid.NewGuid().ToString();
                    content.SetStartTime();
                    using (_logger.BeginScope<Dictionary<string, object>>(content.GetMetaData()))
                    {
                      
                            result = HandleMessageAsync(content, _cancellationToken.Token).Result;
                        
                    }


                    if (result)
                    {
                        _channel.BasicAck(ea.DeliveryTag, false);
                    }
                    else
                    {

                        _channel.BasicNack(ea.DeliveryTag, false, true);
                    }
                

            };

            _consumer.Shutdown += OnConsumerShutdown;
            _consumer.Registered += OnConsumerRegistered;
            _consumer.Unregistered += OnConsumerUnregistered;
            _consumer.ConsumerCancelled += OnConsumerConsumerCancelled;


            _channel.BasicConsume(_rabbitMqQueueConfiguration.QueueName, false, _consumer);
            return Task.CompletedTask;
        }

        public virtual Task<bool> HandleMessageAsync(Message content, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                return _domain.ExecuteAsync(cancellationToken, content);
            }
            catch (Exception)
            {

                return Task.FromResult<bool>(false);
            }
        }
        public abstract void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e);
        public virtual void OnServiceHealthUp(object sender, ServiceHealthEventArgs e)
        {
            //  InitRabbitMQ();
            _channel.BasicConsume(_rabbitMqQueueConfiguration.QueueName, false, _consumer);
            _cancellationToken = new CancellationTokenSource();
        }
        public virtual void OnServiceHealthDegarded(object sender, ServiceHealthEventArgs e)
        {
            _cancellationToken?.Cancel();
      
           
            _channel.BasicCancel(_consumer.ConsumerTag);


        }
        public abstract void OnConsumerUnregistered(object sender, ConsumerEventArgs e);
        public abstract void OnConsumerRegistered(object sender, ConsumerEventArgs e);
        public abstract void OnConsumerShutdown(object sender, ShutdownEventArgs e);
        public abstract void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e);

        public override void Dispose()
        {
            _channel.Close();
            //  _cancellationToken.Dispose();
            _connection.Close();
            base.Dispose();
        }
    }

}
