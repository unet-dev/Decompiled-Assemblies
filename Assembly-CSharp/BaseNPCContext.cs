using Rust.Ai;
using System;

public class BaseNPCContext : BaseContext
{
	public NPCPlayerApex Human;

	public Rust.Ai.AiLocationManager AiLocationManager;

	public BaseNPCContext(IAIAgent agent) : base(agent)
	{
		this.Human = agent as NPCPlayerApex;
	}
}