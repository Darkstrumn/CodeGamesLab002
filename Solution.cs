using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Text;
using System;
class Solution {
    static void TimeConversion(string s) {
        var parts = s.Split(':');
        var hh = Convert.ToInt32(parts[0]);
        var mm = Convert.ToInt32(parts[1]);
        var ssampm = parts[2];
        var pm = (ssampm.Contains("PM") && hh != 12) || (ssampm.Contains("AM") && hh == 12) ? 12 : 0;
        var ss = Convert.ToInt32(ssampm.Remove(2));

        hh = ((hh += pm) % 24);

        Console.WriteLine($"{s} - {hh:00}:{mm:00}:{ss:00}");        
    }

    static int quickestWayUp(int[][] ladders, int[][] snakes) {
        var ret = 0;
        //var dieRolls = 0;
        var sidedDie = 6;
        var gridSize = 100;
        
        var model = new {
            /*virtual game board with snakes and ladders in place (adjusted for 0 index of array space)*/
            /*The scanned paths to goal to map shortest path*/
            //routes = 1 + ladders.Concat(snakes).ToArray().Length  = initial route + the routes introduced by the snakes and ladders
            gamegrid = new int[100]
            .Select((v, i) => new {index = i + 1, value = v, links =  snakes.Concat(ladders).ToArray()})
            .Select(x => (                
                x.links.Where(snakesAndLadders =>  ( snakesAndLadders[0] ) == x.index)
                .Select(snakesAndLadders => snakesAndLadders[1])
                .FirstOrDefault()
                > 0 ?
                    x.links.Where(snakesAndLadders => ( snakesAndLadders[0] ) == x.index)
                    .Select(snakesAndLadders => ( snakesAndLadders[1] ))
                    .FirstOrDefault()
                    : x.index + 1
                ))
            .ToArray()
            .Select((v,i) => new {
                position = i + 1, 
                leadsTo = v % (gridSize + 1),
                numberOfMajorRolls = ( gridSize - ((decimal)i + 1) > sidedDie ? Math.Floor(((decimal)i + 1) / sidedDie) : gridSize - ((decimal)i + 1) == sidedDie ? sidedDie : Math.Floor((gridSize - ((decimal)i + 1)) / sidedDie ) ),
                minorRoll = ( gridSize - ((decimal)i + 1) > sidedDie ? ((decimal)i + 1) % sidedDie :  gridSize - ((decimal)i + 1) )
            }).ToArray(),
            ConstRoute = 0,
            ConstGridPosition = 1,
            ConstNumMajorSidedDie = 2,
            ConstNumMinorSidedDie = 3,
            routes = new int[1][]{new int[1]{1}}
                .Concat(ladders.Concat(snakes))
                .Select(x => new{route = x[0], rollHistory = new List<int>{0} })
                .ToList()
            }; 

        //var startPoint = 0;


        System.Diagnostics.Debug.Print("BP0");


        return ret;
    }

    static long candiesStrategiesStyle(int n, int[] arr) {
        var ret = 0;
        var accumulator = new int[n];
        var model = new {
            candies = new int[n]
            .Select((v, i) => {
                System.Diagnostics.Debug.Print($"------------------\n");
                System.Diagnostics.Debug.Print($"I:{i}, V:{arr[i]}\n");
                System.Diagnostics.Debug.Print($"------------------\n");
                System.Diagnostics.Debug.Print($"prevIndex:{i - (i - 1 >= 0  ? 1 : 0)}\n");
                System.Diagnostics.Debug.Print($"index:{i}\n");
                System.Diagnostics.Debug.Print($"nextIndex:{i + (i + 1 < n  ? 1 : 0)}\n");
                var aliases = new {                    
                    prevIndex = i - (i - 1 >= 0  ? 1 : 0),
                    index = i,
                    nextIndex = i + (i + 1 < n  ? 1 : 0),
                    prevRank = arr[i - (i - 1 >= 0  ? 1 : 0)],
                    rank = arr[i],
                    nextRank = arr[i + (i + 1 < n  ? 1 : 0)],
                    };
                var strategies = (new dynamic[]{
                        new {
                            id = "IsRankingBetterThanNextStudentRankingAndAreTheyLast",
                            rank = aliases.rank,
                            prevRank = aliases.nextRank,
                            isNextRankLastInList = (aliases.nextIndex >= n),
                            triggered = ((aliases.rank > aliases.nextRank) && (aliases.nextIndex + 1 >= n)),
                            reasonRank = (aliases.rank > aliases.nextRank),
                            reasonLast = (aliases.nextIndex + 1 >= n),
                            value = 2
                            },
                        new {
                            id = "IsRankingBetterThanAdjucentStudentRankings",
                            rank = aliases.rank,
                            prevRank = aliases.prevRank,
                            triggered = ((aliases.rank > aliases.prevRank) || (aliases.rank > aliases.nextRank)),
                            reasonRankPrev = (aliases.rank > aliases.prevRank),
                            reasonRankNext = (aliases.rank > aliases.nextRank),
                            value = (1 + (accumulator[aliases.prevIndex] > 0 ? accumulator[aliases.prevIndex] : 1))
                            },
                        new {
                            id = "NoChildLeftBehind",
                            rank = aliases.rank,
                            prevRank = aliases.prevRank,
                            reason = true,
                            triggered = true,
                            value = 1
                            },
                    }).ToList();
                var candies = accumulator[aliases.index];
                foreach(var strategy in strategies)
                {
                    if (strategy.triggered == true)
                    {
                        System.Diagnostics.Debug.Print($"++Strategy: {strategy.id} Accepted.\n");
                        accumulator[aliases.index] = strategy.value;
                        candies = strategy.value;
                        break;
                    } else {
                        System.Diagnostics.Debug.Print($"--Strategy: {strategy.id} Rejected.\n");
                    }
                }
               System.Diagnostics.Debug.Print($"Iteration({aliases.index}):: Ranking:{aliases.rank} - final:{candies}\n");
                return new {
                    candies = candies,
                    aliases = aliases,
                    strategies = strategies
                    };
                })
            //.Select(c => c.candies)
            .ToArray()
            }; 


        //ret = model.candies.Sum();
        System.Diagnostics.Debug.Print($"BPx: {ret}\n");

        return ret;
    }

    static long candies(int n, int[] arr) {
        var ret = 0;
        var streak = 0;
        var model = new {
            candies = new int[n]
            .Select((v, i) => {
                System.Diagnostics.Debug.Print($"------------------\n");
                System.Diagnostics.Debug.Print($"I:{i}, V:{arr[i]}\n");
                System.Diagnostics.Debug.Print($"------------------\n");
                System.Diagnostics.Debug.Print($"prevIndex:{i - (i - 1 >= 0  ? 1 : 0)}\n");
                System.Diagnostics.Debug.Print($"index:{i}\n");
                System.Diagnostics.Debug.Print($"nextIndex:{i + (i + 1 < n  ? 1 : 0)}\n");
                System.Diagnostics.Debug.Print($"0streak:{streak}\n");
                var aliases = new {                    
                    prevIndex = i - (i - 1 >= 0  ? 1 : 0),
                    index = i,
                    nextIndex = i + (i + 1 < n  ? 1 : 0),
                    prevRank = arr[i - (i - 1 >= 0  ? 1 : 0)],
                    rank = arr[i],
                    nextRank = arr[i + (i + 1 < n  ? 1 : 0)],
                    };

                var candies = 1 + ((aliases.rank > aliases.prevRank) ? 1 : (aliases.rank > aliases.nextRank) ? 1 : 0);
                streak += (aliases.rank > aliases.prevRank) && (candies > 1) ? 1 : (streak * -1);
                candies += ((streak > 1) && (candies > 1)) ? streak - 1 : 0;

                System.Diagnostics.Debug.Print($"1streak:{streak}\n");
                System.Diagnostics.Debug.Print($"Iteration({aliases.index}):: Ranking:{aliases.rank} - final:{candies}\n");

                return new {
                    candies = candies,
                    aliases = aliases,
                    streak = streak
                    };
                })
            //.Select(c => c.candies)
            .ToArray()
            }; 


        //ret = model.candies.Sum();
        System.Diagnostics.Debug.Print($"BPx: {ret}\n");

        return ret;
    }

    static string[] FizzBuzz_prototype(int n)
    {
        /*
        This version is the hack, it is short quick and self-documenting
         */
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
        /*
        This version is the SOLID (engineering varient, it is bigger self-documenting, and follows Solid pricipals for future maintainability (single namespace file for ez prototyping, normally would be separate files for each interface and class)
         */
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


    static void NEXT(){

    }

    static void Main(string[] args) {
        var n = 15;
        var result = FizzBuzz_solid(n);
        Console.WriteLine(result);
/*
        var rankings = new int[]{2,4,2,6,1,7,8,9,2,1};
        //                 int[]{1,2,1,2,1,2,3,4,2,1};
        long result = candies(rankings.Length, rankings);
        Console.WriteLine(result.ToString());
*/
 
 /*
        var rankings2 = new int[]{2,4,2,6,1,7,8,9,2,1};
        candies(rankings2.Length, rankings2);
        rankings2 = new int[]{2,4,3,5,2,6,4,5};
        //expected           {1,2,1,2,1,2,1,2}
        candies(rankings2.Length, rankings2);
        rankings2 = new int[]{9,8,5,6,1,7,8,9,2,1};
        //expected           {2,2,1,2,1,2,3,4,2,1}
        candies(rankings2.Length, rankings2);
        rankings2 = new int[]{2,2,2,6,1,7,6,9,2,1};
        //expected           {1,1,1,2,1,2,1,2,2,1}
        candies(rankings2.Length, rankings2);
        rankings2 = new int[]{0,1,0,6,10,7,8,9,2,10};
        //expected           {1,2,1,2,3,1,2,3,1,2}
        candies(rankings2.Length, rankings2);
*/
/*
        var ladders = new int[3][]{
            new int[] {32,62},
            new int[] {42,68},
            new int[] {12,98}
        };
        var snakes =  new int[7][]{
            new int[] {95,13},
            new int[] {97,25},
            new int[] {93,37},
            new int[] {79,27},
            new int[] {75,19},
            new int[] {49,47},
            new int[] {67,17}
        };
        int result = quickestWayUp(ladders, snakes);
        int result = quickestWayUp(ladders, snakes);
        Console.WriteLine(result.ToString());
*/
         //int[] arr = Array.ConvertAll(Console.ReadLine().Split(' '), arrTemp => Convert.ToInt32(arrTemp));
/*
        TimeConversion("07:05:45PM");
        TimeConversion("02:05:45AM");
        TimeConversion("02:05:45PM");
        TimeConversion("12:05:45PM");
        TimeConversion("12:35:59AM");
        TimeConversion("12:00:00PM");
        TimeConversion("12:00:00AM");
*/
    }
}
