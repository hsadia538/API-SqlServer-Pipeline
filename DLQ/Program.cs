using Microsoft.Azure;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using QueueClient = Microsoft.ServiceBus.Messaging.QueueClient;
using SubscriptionClient = Microsoft.ServiceBus.Messaging.SubscriptionClient;

namespace DLQ
{
    class Program
    {

        
        static NamespaceManager nameSpaceManager;

        static void Main(string[] args)
        {
            var MyServiceBus = "Endpoint=sb://servicebuspractice.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=44Xtdc5DTF2dnGjGiRYnYAVfyPVCW3ZVM6boz8Ty/gE=";
            //  string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            MessagingFactory factory = MessagingFactory.CreateFromConnectionString(MyServiceBus);

          //  queueClient = await factory.CreateMessageReceiverAsync(_entityPath, ReceiveMode.ReceiveAndDelete);
            nameSpaceManager = NamespaceManager.CreateFromConnectionString(MyServiceBus);

            var TopicName = "practicetopic";
            //Create Topic
            if (!nameSpaceManager.TopicExists(TopicName))
            {
                nameSpaceManager.CreateTopic(TopicName);
            }
            //nameSpaceManager = NamespaceManager.CreateFromConnectionString(TopicConfigurations.Namespace);
            ReadDLQMessages(TopicName);
            Console.ReadLine();
        }

        public static void ReadDLQMessages(String topicName)
        {
            //Path of Deadletter queue,evey subscripiton has deadletter queue
            var DLQPath = "sb://servicebuspractice.servicebus.windows.net/practicetopic/Subscriptions/sub2/$DeadLetterQueue";

            foreach (SubscriptionDescription description in nameSpaceManager.GetSubscriptions(topicName))
            {
                var Dlqpath = SubscriptionClient.FormatDeadLetterPath(topicName, description.Name);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("=======================================");
                Console.WriteLine("---Order Recieving From DeadLetter Queue [" + Dlqpath + "]---");
                Console.WriteLine("=======================================");

                //here susbcription client is created with deadletter queue
                SubscriptionClient sClient = SubscriptionClient.CreateFromConnectionString("Endpoint=sb://servicebuspractice.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=44Xtdc5DTF2dnGjGiRYnYAVfyPVCW3ZVM6boz8Ty/gE=", topicName, description.Name + DLQPath);
                while (true)
                {
                    BrokeredMessage bmessgage = sClient.Receive(TimeSpan.FromSeconds(1));
                    if (bmessgage != null)
                    {
                        
                        
                        Console.Write($" Request Recieved {bmessgage}");
                       

                        bmessgage.Complete();
                    }
                    else
                    {
                        break;
                    }
                }

                sClient.Close();
            }
        }
    }
}
