namespace AI
{
    public class DecoratorInverter : BehaviorNode
    {
        BehaviorNode _behavior;

        public DecoratorInverter(BehaviorNode behavior)
        {
            _behavior = behavior;
        }

        public override BehaviorResult Behave()
        {
            BehaviorResult result = _behavior.Behave();
            switch(result)
            {
                case BehaviorResult.Success:
                    return BehaviorResult.Failure;
                case BehaviorResult.Failure:
                    return BehaviorResult.Success;
                default:
                    return BehaviorResult.Running;
            }
        }
    }
}
