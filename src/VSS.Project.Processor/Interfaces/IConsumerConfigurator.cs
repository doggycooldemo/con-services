﻿using VSS.Kafka.DotNetClient.Interfaces;
using VSS.Kafka.DotNetClient.Model;

namespace VSS.Project.Processor.Interfaces
{
	public interface IConsumerConfigurator
	{
		IRestProxySettings GetRestProxySettings();
		CreateConsumerRequest GetConsumerRequest();
	}
}