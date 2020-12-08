using System;

namespace TwitterTriggerExtension
{
	public abstract class TwitterTriggerAttributeBase : Attribute
	{
		public string Filter { get; protected set; }
        public string User { get; protected set; }
	}
}