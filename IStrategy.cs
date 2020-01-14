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
                _strategies = System.Reflection.Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .Where(xtype => xtype
                        .GetInterfaces()
                        .Contains(typeof(IStrategy))
                        && !xtype.AssemblyQualifiedName.Contains("BaseStrategy")
                        )
                    .Select( xtype => (IStrategy)( xtype.GetConstructor((System.Type.EmptyTypes)) )?.Invoke(null) )
                        .ToList();

                _strategies.Sort((a, b) => (a.StrategyOrder.CompareTo(b.StrategyOrder)));

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
            return false;
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
                if(strategy.IsTriggered(input)){System.Diagnostics.Debug.Print( $"***** Strategy {strategy.StrategyName} - triggered\n" );}
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
        override public string GetValue(bool truth, int? input = null)
        {
            return (truth) ? StrategyName : string.Empty;
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
        override public string GetValue(bool truth, int? input = null)
        {
            return (truth) ? StrategyName : string.Empty;
        }
    }    
}

namespace DarkTechSystems.Strategies
{
    public class Numbers_Strategy : DefaultStrategy
    {
        public Numbers_Strategy()
        {
            StrategyName = "Numbers";
            StrategyOrder = 2;
        }
        override public bool IsTriggered(int input)
        {
            return (input) % 3 != 0 && (input) % 5 != 0;
        }
        override public string GetValue(bool truth, int? input = null)
        {
            return (truth) ? input.ToString() : string.Empty;
        }
    }
}

/*
--For documentation I put this here as well.
The Prototype version was the basis for the solid type. The prototype fails solid design principles, but also shows why it comes to be.
The SOLID variant shows the con of the strategy pattern in that the code base can be much larger.
The code is technically cleaner but is less concise in this case.

    static string[] FizzBuzz_prototype(int n)
    {
        /--*
        This version is the hack, it is short quick and self-documenting
         *--/
        var ret = new string[n];
        var model = new {
            data = new object[n]
                .Select( (v, i) => {
                    var aliases = new {
                        index = i,
                        value = i + 1,                        
                        //value = (i + 1).ToString()
                        };
                        
                    var strategies = (new dynamic[]{
                        new {
                            strategyId = "Fizz",
                            inputUnderTest = aliases.value,
                            logicResult = (aliases.value) % 3,
                            triggered = (aliases.value) % 3 == 0,
                            value = "Fizz"
                            },
                        new {
                            strategyId = "Buzz",
                            inputUnderTest = aliases.value,
                            logicResult = (aliases.value) % 5,
                            triggered = (aliases.value) % 5 == 0,
                            value = "Buzz"
                            },
                        new {
                            strategyId = "Number",
                            inputUnderTest = aliases.value,
                            logicResult = (aliases.value) % 3 != 0 && (aliases.value) % 5 != 0,
                            triggered = (aliases.value) % 3 != 0 && (aliases.value) % 5 != 0,
                            value = aliases.value
                            },
                    }).ToList();

                    var output = string.Join("", strategies
                        .Select( strategy => {return (strategy.triggered ? strategy.value : "");} )
                        .ToArray());
                        
                    return output;
                })
            .ToArray()
            }; 
            ret = model.data;
        return ret;
    }

    static string[] FizzBuzz_solid(int n)
    {
        /--*
        This version is the SOLID (engineering varient, it is bigger self-documenting, and follows Solid pricipals for future maintainability (single namespace file for ez prototyping, normally would be separate files for each interface and class)
         *--/
        var ret = new string[n];
        var fizzBuzzStrategy = new DarkTechSystems.Strategies.DefaultStrategy();
        var model = new {
            data = new object[n]
                .Select( (v, i) => {
                    var aliases = new {
                        index = i,
                        value = i + 1,                        
                        };
                        
                    var output = fizzBuzzStrategy.ProcessStrategies(aliases.value);
                        
                    return output;
                })
            .ToArray()
            }; 

        ret = model.data;
        return ret;
    }
*/
