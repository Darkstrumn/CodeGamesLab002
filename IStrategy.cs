namespace DarkTechSystems.Strategies
{
    using System.Collections.Generic;
    public interface IStrategy
    {
        string StrategyName {get;}
        int StrategyOrder {get;}
        List<IStrategy> Strategies{get;}
        bool IsTriggered(int input);
        string GetValue(bool truth,int? input = null);
        string GetTriggeredValue(int input);
        string ProcessStrategies(int input);
    }
}

namespace DarkTechSystems.Strategies
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    public class BaseStrategy : IStrategy
    {
        virtual public string StrategyName {get; protected set;}
        virtual public int StrategyOrder {get; protected set;}
        private static List<IStrategy> _strategies;
        virtual public List<IStrategy> Strategies
        {
            get
            {
                var defaultStrategy = System.Reflection.Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .Where(xtype => xtype
                        .GetInterfaces()
                        .Contains(typeof(IStrategy))
                        && xtype.AssemblyQualifiedName.Contains("DefaultStrategy")
                    )
                    .Select( xtype => (IStrategy)( xtype.GetConstructor((System.Type.EmptyTypes)) )?.Invoke(null) )
                    .FirstOrDefault();

                _strategies = System.Reflection.Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .Where(xtype => xtype
                        .GetInterfaces()
                        .Contains(typeof(IStrategy))
                        && !xtype.AssemblyQualifiedName.Contains("BaseStrategy")
                        && !xtype.AssemblyQualifiedName.Contains("DefaultStrategy")
                        )
                    .Select( xtype => (IStrategy)( xtype.GetConstructor((System.Type.EmptyTypes)) )?.Invoke(null) )
                        .ToList();

                _strategies.Add(defaultStrategy);            

                _strategies.Sort((a, b) => (a.StrategyOrder.ToString()[0].CompareTo(b.StrategyOrder.ToString()[0])));;

                return _strategies;
            }
            protected set
            {
                ;
            }
        }
        public BaseStrategy()
        {
            StrategyName = "Base";
            StrategyOrder = 0;
        }
        virtual public bool IsTriggered(int input)
        {
            return true;
        }
        virtual public string GetValue(bool truth,int? input = null)
        {
            return string.Empty;
        }
        virtual public string GetTriggeredValue(int input)
        {
            return GetValue( IsTriggered(input), input );
        }

        virtual public string ProcessStrategies(int input)
        {
            var result = string.Empty;

            foreach ( var item in Strategies.Select( (v, i) => new {index = i, strategy = v} ) )
            {
                var strategy = item.strategy;
                if(strategy.IsTriggered(input)){System.Diagnostics.Debug.Print( $@"{strategy.StrategyName}" );}
                result += strategy.GetTriggeredValue(input);
            }

            return result;
        }
    }
}

namespace DarkTechSystems.Strategies
{
    using System;
    using System.Linq;
    public class DefaultStrategy : BaseStrategy
    {
        public DefaultStrategy() : base()
        {
            StrategyName = "Default";
            StrategyOrder = int.MaxValue;
        }
    }
}

namespace DarkTechSystems.Strategies
{
    public class Numbers_Strategy : DefaultStrategy
    {
        public Numbers_Strategy()
        {
            StrategyName = "numbers";
            StrategyOrder = 2;
        }
        override public bool IsTriggered(int input)
        {
            return (input) % 3 != 0 && (input) % 5 != 0;
        }
        override public string GetValue(bool truth,int? input = null)
        {
            return (truth) ? input.ToString() : string.Empty;
        }
    }
}

namespace DarkTechSystems.Strategies
{
    public class Fizz_Strategy : DefaultStrategy
    {
        public Fizz_Strategy()
        {
            StrategyName = "Fizz";
            StrategyOrder = 0;
        }
        override public bool IsTriggered(int input)
        {
            return ((input) % 3 == 0 );
        }
        override public string GetValue(bool truth,int? input = null)
        {
            return (truth) ? "Fizz" : string.Empty;
        }
    }
}

namespace DarkTechSystems.Strategies
{
    public class Buzz_Strategy : DefaultStrategy
    {
        public Buzz_Strategy()
        {
            StrategyName = "Buzz";
            StrategyOrder = 1;
        }
        override public bool IsTriggered(int input)
        {
            return ((input) % 5 == 0 );
        }
        override public string GetValue(bool truth,int? input = null)
        {
            return (truth) ? "Buzz" : string.Empty;
        }
    }    
}
