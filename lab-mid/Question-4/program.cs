using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GrammarAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Grammar Analysis - FIRST and FOLLOW Sets");
            Console.WriteLine("=======================================");
            Console.WriteLine("Given grammar:");
            Console.WriteLine("E → T X");
            Console.WriteLine("X → + T X | ε");
            Console.WriteLine("T → int | ( E )");
            Console.WriteLine();

            // Create grammar with hardcoded rules
            var grammar = new Grammar();
            grammar.AddRule("E → T X");
            grammar.AddRule("X → + T X | ε");
            grammar.AddRule("T → int | ( E )");

            // Validate and compute FIRST and FOLLOW sets
            try
            {
                // Check for left recursion and ambiguity
                if (grammar.HasLeftRecursion())
                {
                    Console.WriteLine("Grammar invalid for top-down parsing.");
                    Console.WriteLine("Reason: Left recursion detected.");
                    return;
                }

                if (grammar.HasAmbiguity())
                {
                    Console.WriteLine("Grammar invalid for top-down parsing.");
                    Console.WriteLine("Reason: Ambiguity detected.");
                    return;
                }

                Console.WriteLine("Grammar is valid for top-down parsing (no left recursion or ambiguity).");

                // Compute FIRST sets
                var firstSets = grammar.ComputeFirstSets();

                Console.WriteLine("\nFIRST Sets:");
                foreach (var entry in firstSets)
                {
                    // Only show non-terminals
                    if (grammar.IsNonTerminal(entry.Key))
                    {
                        Console.WriteLine($"FIRST({entry.Key}) = {{{string.Join(", ", entry.Value)}}}");
                    }
                }

                // Compute FOLLOW sets
                var followSets = grammar.ComputeFollowSets();

                Console.WriteLine("\nFOLLOW Sets:");
                foreach (var entry in followSets)
                {
                    Console.WriteLine($"FOLLOW({entry.Key}) = {{{string.Join(", ", entry.Value)}}}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during analysis: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }

    class Grammar
    {
        private Dictionary<string, List<List<string>>> productions;
        private HashSet<string> nonTerminals;
        private HashSet<string> terminals;
        private string startSymbol;

        public Grammar()
        {
            productions = new Dictionary<string, List<List<string>>>();
            nonTerminals = new HashSet<string>();
            terminals = new HashSet<string>();
            startSymbol = null;
        }

        public bool IsNonTerminal(string symbol)
        {
            return nonTerminals.Contains(symbol);
        }

        public void AddRule(string rule)
        {
            // Parse rule in format "A → B C | D"
            var parts = rule.Split(new[] { '→', '-', '>' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
                throw new FormatException("Rule must be in the format 'A → B C | D'");

            string leftSide = parts[0].Trim();
            string rightSide = parts[1].Trim();

            // Add non-terminal
            nonTerminals.Add(leftSide);

            // Set start symbol if first rule
            if (startSymbol == null)
                startSymbol = leftSide;

            // Parse productions (alternatives)
            var alternatives = rightSide.Split('|');
            var productionList = new List<List<string>>();

            foreach (var alt in alternatives)
            {
                string trimmedAlt = alt.Trim();

                // Handle epsilon (empty) production
                if (trimmedAlt == "ε" || trimmedAlt == "")
                {
                    productionList.Add(new List<string> { "ε" });
                    continue;
                }

                // Split into symbols
                var symbols = Regex.Matches(trimmedAlt, @"(?:\w+|\([^)]*\)|\S)")
                                  .Cast<Match>()
                                  .Select(m => m.Value.Trim())
                                  .Where(s => !string.IsNullOrWhiteSpace(s))
                                  .ToList();

                foreach (var symbol in symbols)
                {
                    if (!nonTerminals.Contains(symbol) && symbol != "ε")
                        terminals.Add(symbol);
                }

                productionList.Add(symbols);
            }

            productions[leftSide] = productionList;
        }

        public bool HasLeftRecursion()
        {
            foreach (var nt in nonTerminals)
            {
                if (HasDirectLeftRecursion(nt) || HasIndirectLeftRecursion(nt, new HashSet<string>()))
                    return true;
            }
            return false;
        }

        private bool HasDirectLeftRecursion(string nonTerminal)
        {
            if (!productions.ContainsKey(nonTerminal))
                return false;

            foreach (var production in productions[nonTerminal])
            {
                if (production.Count > 0 && production[0] == nonTerminal)
                    return true;
            }
            return false;
        }

        private bool HasIndirectLeftRecursion(string nonTerminal, HashSet<string> visited)
        {
            if (visited.Contains(nonTerminal))
                return false; // Already checked this path

            visited.Add(nonTerminal);

            if (!productions.ContainsKey(nonTerminal))
                return false;

            foreach (var production in productions[nonTerminal])
            {
                if (production.Count > 0 && nonTerminals.Contains(production[0]) &&
                    (HasDirectLeftRecursion(production[0]) || HasIndirectLeftRecursion(production[0], new HashSet<string>(visited))))
                    return true;
            }
            return false;
        }

        public bool HasAmbiguity()
        {
            // For this specific grammar analysis, we'll implement a simplified check
            // that looks for common prefixes in productions of the same non-terminal

            foreach (var nt in nonTerminals)
            {
                if (!productions.ContainsKey(nt) || productions[nt].Count <= 1)
                    continue;

                var firstForProductions = new HashSet<string>[productions[nt].Count];

                for (int i = 0; i < productions[nt].Count; i++)
                {
                    firstForProductions[i] = ComputeFirstOfString(productions[nt][i]);
                }

                // Check for common elements in FIRST sets of different productions
                for (int i = 0; i < firstForProductions.Length; i++)
                {
                    for (int j = i + 1; j < firstForProductions.Length; j++)
                    {
                        if (firstForProductions[i].Overlaps(firstForProductions[j]))
                            return true;
                    }
                }
            }
            return false;
        }

        public Dictionary<string, HashSet<string>> ComputeFirstSets()
        {
            var firstSets = new Dictionary<string, HashSet<string>>();

            // Initialize FIRST sets
            foreach (var terminal in terminals)
            {
                firstSets[terminal] = new HashSet<string> { terminal };
            }

            foreach (var nonTerminal in nonTerminals)
            {
                firstSets[nonTerminal] = new HashSet<string>();
            }

            bool changed;
            do
            {
                changed = false;

                foreach (var nt in nonTerminals)
                {
                    if (!productions.ContainsKey(nt)) continue;

                    foreach (var production in productions[nt])
                    {
                        // For each production A → α
                        if (production.Count == 0 || production[0] == "ε")
                        {
                            // If α is ε, add ε to FIRST(A)
                            if (firstSets[nt].Add("ε"))
                                changed = true;
                            continue;
                        }

                        bool allDeriveEpsilon = true;
                        for (int i = 0; i < production.Count; i++)
                        {
                            string symbol = production[i];
                            bool symbolDerivesEpsilon = false;

                            if (!firstSets.ContainsKey(symbol))
                            {
                                // Unknown symbol - could be terminal or forgotten non-terminal
                                Console.WriteLine($"Warning: Symbol '{symbol}' not recognized");
                                continue;
                            }

                            // Add all elements from FIRST(symbol) except ε to FIRST(nt)
                            foreach (var terminal in firstSets[symbol].Where(s => s != "ε"))
                            {
                                if (firstSets[nt].Add(terminal))
                                    changed = true;
                            }

                            // Check if symbol can derive ε
                            symbolDerivesEpsilon = firstSets[symbol].Contains("ε");

                            if (!symbolDerivesEpsilon)
                            {
                                allDeriveEpsilon = false;
                                break;
                            }
                        }

                        // If all symbols in production can derive ε, add ε to FIRST(nt)
                        if (allDeriveEpsilon)
                        {
                            if (firstSets[nt].Add("ε"))
                                changed = true;
                        }
                    }
                }
            } while (changed);

            return firstSets;
        }

        private HashSet<string> ComputeFirstOfString(List<string> symbols)
        {
            var firstSet = new HashSet<string>();

            if (symbols.Count == 0 || symbols[0] == "ε")
            {
                firstSet.Add("ε");
                return firstSet;
            }

            var firstSets = ComputeFirstSets();
            bool allDeriveEpsilon = true;

            for (int i = 0; i < symbols.Count; i++)
            {
                string symbol = symbols[i];

                if (!firstSets.ContainsKey(symbol))
                    continue;

                // Add all elements except ε from FIRST(symbol)
                foreach (var terminal in firstSets[symbol].Where(s => s != "ε"))
                {
                    firstSet.Add(terminal);
                }

                // If this symbol cannot derive ε, we're done
                if (!firstSets[symbol].Contains("ε"))
                {
                    allDeriveEpsilon = false;
                    break;
                }
            }

            // If all symbols can derive ε, add ε to the result
            if (allDeriveEpsilon)
            {
                firstSet.Add("ε");
            }

            return firstSet;
        }

        public Dictionary<string, HashSet<string>> ComputeFollowSets()
        {
            var followSets = new Dictionary<string, HashSet<string>>();
            var firstSets = ComputeFirstSets();

            // Initialize FOLLOW sets
            foreach (var nonTerminal in nonTerminals)
            {
                followSets[nonTerminal] = new HashSet<string>();
            }

            // Add end marker '$' to FOLLOW of start symbol
            followSets[startSymbol].Add("$");

            bool changed;
            do
            {
                changed = false;

                foreach (var nt in nonTerminals)
                {
                    if (!productions.ContainsKey(nt)) continue;

                    foreach (var production in productions[nt])
                    {
                        for (int i = 0; i < production.Count; i++)
                        {
                            string symbol = production[i];

                            // Only compute FOLLOW for non-terminals
                            if (!nonTerminals.Contains(symbol))
                                continue;

                            // What follows this symbol in the production
                            bool allRemainingDeriveEpsilon = true;

                            if (i < production.Count - 1)
                            {
                                // There are symbols after the current one
                                allRemainingDeriveEpsilon = true;

                                // Compute FIRST of remaining string β in A → αBβ
                                var remainingSymbols = production.Skip(i + 1).ToList();
                                var firstOfRemaining = ComputeFirstOfString(remainingSymbols);

                                // Add all terminals from FIRST(β) to FOLLOW(B)
                                foreach (var terminal in firstOfRemaining.Where(s => s != "ε"))
                                {
                                    if (followSets[symbol].Add(terminal))
                                        changed = true;
                                }

                                // Check if β can derive ε
                                allRemainingDeriveEpsilon = firstOfRemaining.Contains("ε");
                            }

                            // If all remaining symbols derive ε or we're at the end,
                            // add all from FOLLOW(nt) to FOLLOW(symbol)
                            if (i == production.Count - 1 || allRemainingDeriveEpsilon)
                            {
                                foreach (var terminal in followSets[nt])
                                {
                                    if (followSets[symbol].Add(terminal))
                                        changed = true;
                                }
                            }
                        }
                    }
                }
            } while (changed);

            return followSets;
        }
    }
}