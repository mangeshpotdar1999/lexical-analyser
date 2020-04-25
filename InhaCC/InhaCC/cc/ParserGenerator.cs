

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParserGenerator
{
    public class ParserProduction
    {
        public int index;
        public string production_name;
        public bool isterminal;
        public List<ParserProduction> contents = new List<ParserProduction>();
        public List<List<ParserProduction>> sub_productions = new List<List<ParserProduction>>();

        public static ParserProduction operator +(ParserProduction p1, ParserProduction p2)
        {
            p1.contents.Add(p2);
            return p1;
        }

        public static ParserProduction operator |(ParserProduction p1, ParserProduction p2)
        {
            p2.contents.Insert(0, p2);
            p1.sub_productions.Add(new List<ParserProduction>(p2.contents));
            p2.contents.Clear();
            return p1;
        }

#if false
        public static ParserProduction operator +(ParserProduction p1, string p2)
        {
            p1.contents.Add(new ParserProduction { isterminal = true, token_specific = p2 });
            return p1;
        }

        public static ParserProduction operator|(ParserProduction p1, string p2)
        {
            p1.sub_productions.Add(new List<ParserProduction> { p1, new ParserProduction { isterminal = true, token_specific = p2 } });
            return p1;
        }
#endif
    }

    /// <summary>
    /// LR Parser Generator
    /// </summary>
    public class ParserGenerator
    {
        List<ParserProduction> production_rules;
        // (production_index, (priority, is_left_associativity?))
        Dictionary<int, Tuple<int, bool>> shift_reduce_conflict_solve;
        // (production_index, (sub_production_index, (priority, is_left_associativity?)))
        Dictionary<int, Dictionary<int, Tuple<int, bool>>> shift_reduce_conflict_solve_with_production_rule;

        public StringBuilder GlobalPrinter = new StringBuilder();

        public readonly static ParserProduction EmptyString = new ParserProduction { index = -2 };

        public ParserGenerator()
        {
            production_rules = new List<ParserProduction>();
            production_rules.Add(new ParserProduction { index = 0, production_name = "S'" });
            shift_reduce_conflict_solve = new Dictionary<int, Tuple<int, bool>>();
            shift_reduce_conflict_solve_with_production_rule = new Dictionary<int, Dictionary<int, Tuple<int, bool>>>();
        }

        public ParserProduction CreateNewProduction(string name = "", bool is_terminal = true)
        {
            var pp = new ParserProduction { index = production_rules.Count, production_name = name, isterminal = is_terminal };
            production_rules.Add(pp);
            return pp;
        }

        public void PushStarts(ParserProduction pp)
        {
            // Augment stats node
            production_rules[0].sub_productions.Add(new List<ParserProduction> { pp });
        }

        public void PushConflictSolver(bool left, params ParserProduction[] terminals)
        {
            var priority = shift_reduce_conflict_solve.Count + shift_reduce_conflict_solve_with_production_rule.Count;
            foreach (var pp in terminals)
                shift_reduce_conflict_solve.Add(pp.index, new Tuple<int, bool>(priority, left));
        }

        public void PushConflictSolver(bool left, params Tuple<ParserProduction, int>[] no)
        {
            var priority = shift_reduce_conflict_solve.Count + shift_reduce_conflict_solve_with_production_rule.Count;
            foreach (var ppi in no)
            {
                if (!shift_reduce_conflict_solve_with_production_rule.ContainsKey(ppi.Item1.index))
                    shift_reduce_conflict_solve_with_production_rule.Add(ppi.Item1.index, new Dictionary<int, Tuple<int, bool>>());
                shift_reduce_conflict_solve_with_production_rule[ppi.Item1.index].Add(ppi.Item2, new Tuple<int, bool>(priority, left));
            }
        }

        #region String Hash Function
        private string t2s(Tuple<int, int, int> t)
        {
            return $"{t.Item1},{t.Item2},{t.Item3}";
        }

        private string t2s(Tuple<int, int, int, HashSet<int>> t)
        {
            var list = t.Item4.ToList();
            list.Sort();
            return $"{t.Item1},{t.Item2},{t.Item3},({string.Join(",", list)})";
        }

        private string l2s(List<Tuple<int, int, int>> h)
        {
            var list = h.ToList();
            list.Sort();
            return string.Join(",", list.Select(x => $"({x.Item1},{x.Item2},{x.Item3})"));
        }

        private string l2s(List<Tuple<int, int, int, HashSet<int>>> h)
        {
            var list = new List<Tuple<int, int, int, List<int>>>();
            foreach (var tt in h)
            {
                var ll = tt.Item4.ToList();
                ll.Sort();
                list.Add(new Tuple<int, int, int, List<int>>(tt.Item1, tt.Item2, tt.Item3, ll));
            }
            list.Sort();
            return string.Join(",", list.Select(x => $"({x.Item1},{x.Item2},{x.Item3},({(string.Join("/", x.Item4))}))"));
        }

        private string i2s(int a, int b, int c)
        {
            return $"{a},{b},{c}";
        }
        #endregion

        private void print_hs(List<HashSet<int>> lhs, string prefix)
        {
            for (int i = 0; i < lhs.Count; i++)
                if (lhs[i].Count > 0)
                    GlobalPrinter.Append(
                        $"{prefix}({production_rules[i].production_name})={{{string.Join(",", lhs[i].ToList().Select(x => x == -1 ? "$" : production_rules[x].production_name))}}}\r\n");
        }

        private void print_header(string head_text)
        {
            GlobalPrinter.Append("\r\n" + new string('=', 50) + "\r\n\r\n");
            int spaces = 50 - head_text.Length;
            int padLeft = spaces / 2 + head_text.Length;
            GlobalPrinter.Append(head_text.PadLeft(padLeft).PadRight(50));
            GlobalPrinter.Append("\r\n\r\n" + new string('=', 50) + "\r\n");
        }

        private void print_states(int state, List<Tuple<int, int, int, HashSet<int>>> items)
        {
            var builder = new StringBuilder();
            builder.Append("-----" + "I" + state + "-----\r\n");

            foreach (var item in items)
            {
                builder.Append($"{production_rules[item.Item1].production_name.ToString().PadLeft(10)} -> ");

                var builder2 = new StringBuilder();
                for (int i = 0; i < production_rules[item.Item1].sub_productions[item.Item2].Count; i++)
                {
                    if (i == item.Item3)
                        builder2.Append("·");
                    builder2.Append(production_rules[item.Item1].sub_productions[item.Item2][i].production_name + " ");
                    if (item.Item3 == production_rules[item.Item1].sub_productions[item.Item2].Count && i == item.Item3 - 1)
                        builder2.Append("·");
                }
                builder.Append(builder2.ToString().PadRight(30));

                builder.Append($"{string.Join("/", item.Item4.ToList().Select(x => x == -1 ? "$" : production_rules[x].production_name))}\r\n");
            }

            GlobalPrinter.Append(builder.ToString());
        }

        private void print_merged_states(int state, List<Tuple<int, int, int, HashSet<int>>> items, List<List<List<int>>> external_gotos)
        {
            var builder = new StringBuilder();
            builder.Append("-----" + "I" + state + "-----\r\n");

            for (int j = 0; j < items.Count; j++)
            {
                var item = items[j];

                builder.Append($"{production_rules[item.Item1].production_name.ToString().PadLeft(10)} -> ");

                var builder2 = new StringBuilder();
                for (int i = 0; i < production_rules[item.Item1].sub_productions[item.Item2].Count; i++)
                {
                    if (i == item.Item3)
                        builder2.Append("·");
                    builder2.Append(production_rules[item.Item1].sub_productions[item.Item2][i].production_name + " ");
                    if (item.Item3 == production_rules[item.Item1].sub_productions[item.Item2].Count && i == item.Item3 - 1)
                        builder2.Append("·");
                }
                builder.Append(builder2.ToString().PadRight(30));

                builder.Append($"[{string.Join("/", item.Item4.ToList().Select(x => x == -1 ? "$" : production_rules[x].production_name))}] ");
                for (int i = 0; i < external_gotos.Count; i++)
                    builder.Append($"[{string.Join("/", external_gotos[i][j].ToList().Select(x => x == -1 ? "$" : production_rules[x].production_name))}] ");
                builder.Append("\r\n");
            }

            GlobalPrinter.Append(builder.ToString());
        }

        int number_of_states = -1;
        Dictionary<int, List<Tuple<int, int>>> shift_info;
        Dictionary<int, List<Tuple<int, int, int>>> reduce_info;



        public void PrintStates()
        {
            print_header("FINAL STATES");
            for (int i = 0; i < number_of_states; i++)
            {
                var builder = new StringBuilder();
                var x = $"I{i} => ";
                builder.Append(x);
                if (shift_info.ContainsKey(i))
                {
                    builder.Append("SHIFT{" + string.Join(",", shift_info[i].Select(y => $"({production_rules[y.Item1].production_name},I{y.Item2})")) + "}");
                    if (reduce_info.ContainsKey(i))
                        builder.Append("\r\n" + "".PadLeft(x.Length) + "REDUCE{" + string.Join(",", reduce_info[i].Select(y => $"({(y.Item1 == -1 ? "$" : production_rules[y.Item1].production_name)},{(y.Item2 == 0 ? "accept" : production_rules[y.Item2].production_name)},{y.Item3})")) + "}");
                }
                else if (reduce_info.ContainsKey(i))
                    builder.Append("REDUCE{" + string.Join(",", reduce_info[i].Select(y => $"({(y.Item1 == -1 ? "$" : production_rules[y.Item1].production_name)},{(y.Item2 == 0 ? "accept" : production_rules[y.Item2].production_name)},{y.Item3})")) + "}");
                GlobalPrinter.Append(builder.ToString() + "\r\n");
            }
        }

        public void PrintTable()
        {
            var production_mapping = new List<List<int>>();
            var pm_count = 0;

            foreach (var pr in production_rules)
            {
                var pm = new List<int>();
                foreach (var sub_pr in pr.sub_productions)
                    pm.Add(pm_count++);
                production_mapping.Add(pm);
            }

            var builder = new StringBuilder();

            var tokens = new Dictionary<int, int>();
            var max_len = 0;
            foreach (var pp in production_rules)
                if (pp.isterminal)
                    tokens.Add(tokens.Count, pp.index);
            tokens.Add(tokens.Count, -1);
            foreach (var pp in production_rules)
            {
                if (pp.index == 0) continue;
                if (!pp.isterminal)
                    tokens.Add(tokens.Count, pp.index);
                max_len = Math.Max(max_len, pp.production_name.Length);
            }

            var split_line = "+" + new string('*', production_rules.Count + 1).Replace("*", new string('-', max_len + 2) + "+") + "\r\n";
            builder.Append(split_line);

            // print production rule
            builder.Append('|' + "".PadLeft(max_len + 2) + '|');
            for (int i = 0; i < tokens.Count; i++)
            {
                builder.Append(" " + (tokens[i] == -1 ? "$" : production_rules[tokens[i]].production_name).PadLeft(max_len) + " ");
                builder.Append('|');
            }
            builder.Append("\r\n");
            builder.Append(split_line);

            // print states
            for (int i = 0; i < number_of_states; i++)
            {
                builder.Append('|' + "  " + $"{i}".PadLeft(max_len - 2) + "  |");

                // (what, (state_index, isshift))
                var sr_info = new Dictionary<int, Tuple<int, bool>>();

                if (shift_info.ContainsKey(i))
                {
                    foreach (var si in shift_info[i])
                        if (!sr_info.ContainsKey(si.Item1))
                            sr_info.Add(si.Item1, new Tuple<int, bool>(si.Item2, true));
                }
                if (reduce_info.ContainsKey(i))
                {
                    foreach (var ri in reduce_info[i])
                        if (!sr_info.ContainsKey(ri.Item1))
                            sr_info.Add(ri.Item1, new Tuple<int, bool>(production_mapping[ri.Item2][ri.Item3], false));
                }

                for (int j = 0; j < tokens.Count; j++)
                {
                    var k = tokens[j];
                    if (sr_info.ContainsKey(k))
                    {
                        var ss = "";
                        if (sr_info[k].Item2)
                        {
                            if (production_rules[k].isterminal)
                                ss += "s" + sr_info[k].Item1;
                            else
                                ss = sr_info[k].Item1.ToString();
                        }
                        else
                        {
                            if (sr_info[k].Item1 == 0)
                                ss += "acc";
                            else
                                ss += "r" + sr_info[k].Item1;
                        }
                        builder.Append(" " + ss.PadLeft(max_len) + " |");
                    }
                    else
                    {
                        builder.Append("".PadLeft(max_len+2) + "|");
                    }
                }

                builder.Append("\r\n");
            }
            builder.Append(split_line);
            
            print_header("PARSING TABLE");
            GlobalPrinter.Append(builder.ToString() + "\r\n");
        }

        /// <summary>
        /// Calculate FIRST only Terminals
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private HashSet<int> first_terminals(int index)
        {
            var result = new HashSet<int>();
            var q = new Queue<int>();
            var visit = new List<bool>();
            visit.AddRange(Enumerable.Repeat(false, production_rules.Count));
            q.Enqueue(index);

            while (q.Count != 0)
            {
                var p = q.Dequeue();
                if (p < 0 || visit[p]) continue;
                visit[p] = true;

                if (p < 0 || production_rules[p].isterminal)
                    result.Add(p);
                else
                    production_rules[p].sub_productions.Where(x => x.Count > 0).ToList().ForEach(y => q.Enqueue(y[0].index));
            }

            return result;
        }

        /// <summary>
        /// Calculate FIRST only Non-Terminals
        /// </summary>
        /// <param name="production_rule_index"></param>
        /// <returns></returns>
        private List<Tuple<int, int, int>> first_nonterminals(int production_rule_index)
        {
            // (production_rule_index, sub_productions_pos, dot_position)
            var first_l = new List<Tuple<int, int, int>>();
            // (production_rule_index, sub_productions_pos)
            var first_q = new Queue<Tuple<int, int>>();
            // (production_rule_index, (sub_productions_pos))
            var first_visit = new Dictionary<int, HashSet<int>>();
            first_q.Enqueue(new Tuple<int, int>(production_rule_index, 0));
            for (int j = 0; j < production_rules[production_rule_index].sub_productions.Count; j++)
                first_q.Enqueue(new Tuple<int, int>(production_rule_index, j));
            // Get all of starts node FIRST non-terminals
            while (first_q.Count != 0)
            {
                var t = first_q.Dequeue();
                if (first_visit.ContainsKey(t.Item1) && first_visit[t.Item1].Contains(t.Item2)) continue;
                if (!first_visit.ContainsKey(t.Item1)) first_visit.Add(t.Item1, new HashSet<int>());
                first_visit[t.Item1].Add(t.Item2);
                first_l.Add(new Tuple<int, int, int>(t.Item1, t.Item2, 0));
                for (int i = 0; i < production_rules[t.Item1].sub_productions.Count; i++)
                {
                    var sub_pr = production_rules[t.Item1].sub_productions[i];
                    if (sub_pr[0].isterminal == false)
                        for (int j = 0; j < production_rules[sub_pr[0].index].sub_productions.Count; j++)
                            first_q.Enqueue(new Tuple<int, int>(sub_pr[0].index, j));
                }
            }
            return first_l;
        }

        /// <summary>
        /// Get lookahead states item with first item's closure
        /// This function is used in first_with_lookahead function. 
        /// -1: Sentinel lookahead
        /// </summary>
        /// <param name="production_rule_index"></param>
        /// <returns></returns>
        private List<Tuple<int, int, int, HashSet<int>>> lookahead_with_first(int production_rule_index, int sub_production, int sub_production_index, HashSet<int> pred)
        {
            // (production_rule_index, sub_productions_pos, dot_position, (lookahead))
            var states = new List<Tuple<int, int, int, HashSet<int>>>();
            // (production_rule_index, (sub_productions_pos))
            var first_visit = new Dictionary<int, HashSet<int>>();
            states.Add(new Tuple<int, int, int, HashSet<int>>(production_rule_index, sub_production, sub_production_index, pred));
            if (production_rule_index == 0 && sub_production == 0 && sub_production_index == 0)
                pred.Add(-1); // Push sentinel
            if (production_rules[production_rule_index].sub_productions[sub_production].Count > sub_production_index)
            {
                if (!production_rules[production_rule_index].sub_productions[sub_production][sub_production_index].isterminal)
                {
                    var index_populate = production_rules[production_rule_index].sub_productions[sub_production][sub_production_index].index;
                    if (production_rules[production_rule_index].sub_productions[sub_production].Count <= sub_production_index + 1)
                    {
                        for (int i = 0; i < production_rules[index_populate].sub_productions.Count; i++)
                            states.Add(new Tuple<int, int, int, HashSet<int>>(index_populate, i, 0, new HashSet<int>(pred)));
                    }
                    else
                    {
                        var first_lookahead = first_terminals(production_rules[production_rule_index].sub_productions[sub_production][sub_production_index + 1].index);
                        for (int i = 0; i < production_rules[index_populate].sub_productions.Count; i++)
                            states.Add(new Tuple<int, int, int, HashSet<int>>(index_populate, i, 0, new HashSet<int>(first_lookahead)));
                    }
                }
            }
            return states;
        }

        /// <summary>
        /// Get FIRST items with lookahead (Build specific states completely)
        /// </summary>
        /// <param name="production_rule_index"></param>
        /// <param name="sub_production"></param>
        /// <param name="sub_production_index"></param>
        /// <param name="pred"></param>
        /// <returns></returns>
        private List<Tuple<int, int, int, HashSet<int>>> first_with_lookahead(int production_rule_index, int sub_production, int sub_production_index, HashSet<int> pred)
        {
            // (production_rule_index, sub_productions_pos, dot_position, (lookahead))
            var states = new List<Tuple<int, int, int, HashSet<int>>>();
            // (production_rule_index + sub_productions_pos + dot_position), (states_index)
            var states_prefix = new Dictionary<string, int>();

            var q = new Queue<List<Tuple<int, int, int, HashSet<int>>>>();
            q.Enqueue(lookahead_with_first(production_rule_index, sub_production, sub_production_index, pred));
            while (q.Count != 0)
            {
                var ll = q.Dequeue();
                foreach (var e in ll)
                {
                    var ii = i2s(e.Item1, e.Item2, e.Item3);
                    if (!states_prefix.ContainsKey(ii))
                    {
                        states_prefix.Add(ii, states.Count);
                        states.Add(e);
                        q.Enqueue(lookahead_with_first(e.Item1, e.Item2, e.Item3, e.Item4));
                    }
                    else
                    {
                        foreach (var hse in e.Item4)
                            states[states_prefix[ii]].Item4.Add(hse);
                    }
                }
            }

            // (production_rule_index + sub_productions_pos + dot_position), (states_index)
            var states_prefix2 = new Dictionary<string, int>();
            var states_count = 0;
            bool change = false;

            do
            {
                change = false;
                q.Enqueue(lookahead_with_first(production_rule_index, sub_production, sub_production_index, pred));
                while (q.Count != 0)
                {
                    var ll = q.Dequeue();
                    foreach (var e in ll)
                    {
                        var ii = i2s(e.Item1, e.Item2, e.Item3);
                        if (!states_prefix2.ContainsKey(ii))
                        {
                            states_prefix2.Add(ii, states_count);
                            foreach (var hse in e.Item4)
                                if (!states[states_prefix[ii]].Item4.Contains(hse))
                                {
                                    change = true;
                                    states[states_prefix[ii]].Item4.Add(hse);
                                }
                            q.Enqueue(lookahead_with_first(e.Item1, e.Item2, e.Item3, states[states_count].Item4));
                            states_count++;
                        }
                        else
                        {
                            foreach (var hse in e.Item4)
                                if (!states[states_prefix[ii]].Item4.Contains(hse))
                                {
                                    change = true;
                                    states[states_prefix[ii]].Item4.Add(hse);
                                }
                        }
                    }
                }
            } while (change);

            return states;
        }

        private ParserProduction get_first_on_right_terminal(ParserProduction pp, int sub_production)
        {
            for (int i = pp.sub_productions[sub_production].Count - 1; i >= 0; i--)
                if (pp.sub_productions[sub_production][i].isterminal)
                    return pp.sub_productions[sub_production][i];
            throw new Exception($"Cannot solve shift-reduce conflict!\r\nProduction Name: {pp.production_name}\r\nProduction Index: {sub_production}");
        }

        /// <summary>
        /// Create ShiftReduce Parser
        /// </summary>
        /// <returns></returns>
        public ShiftReduceParser CreateShiftReduceParserInstance()
        {
            var symbol_table = new Dictionary<string, int>();
            var jump_table = new int[number_of_states][];
            var goto_table = new int[number_of_states][];
            var grammar = new List<List<int>>();
            var grammar_group = new List<int>();
            var production_mapping = new List<List<int>>();
            var pm_count = 0;

            foreach (var pr in production_rules)
            {
                var ll = new List<List<int>>();
                var pm = new List<int>();
                foreach (var sub_pr in pr.sub_productions)
                {
                    ll.Add(sub_pr.Select(x => x.index).ToList());
                    pm.Add(pm_count++);
                    grammar_group.Add(production_mapping.Count);
                }
                grammar.AddRange(ll);
                production_mapping.Add(pm);
            }

            for (int i = 0; i < number_of_states; i++)
            {
                // Last elements is sentinel
                jump_table[i] = new int[production_rules.Count + 1];
                goto_table[i] = new int[production_rules.Count + 1];
            }

            foreach (var pr in production_rules)
                symbol_table.Add(pr.production_name ?? "^", pr.index);
            symbol_table.Add("$", production_rules.Count);

            foreach (var shift in shift_info)
                foreach (var elem in shift.Value)
                {
                    jump_table[shift.Key][elem.Item1] = 1;
                    goto_table[shift.Key][elem.Item1] = elem.Item2;
                }

            foreach (var reduce in reduce_info)
                foreach (var elem in reduce.Value)
                {
                    var index = elem.Item1;
                    if (index == -1) index = production_rules.Count;
                    if (jump_table[reduce.Key][index] != 0)
                        throw new Exception($"Error! Shift-Reduce Conflict is not solved! Please use LALR or LR(1) parser!\r\nJump-Table: {reduce.Key} {index}");
                    if (elem.Item2 == 0)
                        jump_table[reduce.Key][index] = 3;
                    else
                    {
                        jump_table[reduce.Key][index] = 2;
                        goto_table[reduce.Key][index] = production_mapping[elem.Item2][elem.Item3];
                    }
                }

            return new ShiftReduceParser(symbol_table, jump_table, goto_table, grammar_group.ToArray(), grammar.Select(x => x.ToArray()).ToArray());
        }
    }

    public class ParsingTree
    {
        public class ParsingTreeNode
        {
            public string Produnction;
            public string Contents;
            public object UserContents;
            public int ProductionRuleIndex;
            public ParsingTreeNode Parent;
            public List<ParsingTreeNode> Childs;

            public static ParsingTreeNode NewNode()
                => new ParsingTreeNode { Parent = null, Childs = new List<ParsingTreeNode>() };
            public static ParsingTreeNode NewNode(string production)
                => new ParsingTreeNode { Parent = null, Childs = new List<ParsingTreeNode>(), Produnction = production };
            public static ParsingTreeNode NewNode(string production, string contents)
                => new ParsingTreeNode { Parent = null, Childs = new List<ParsingTreeNode>(), Produnction = production, Contents = contents };
        }
        
        ParsingTreeNode root;

        public ParsingTree(ParsingTreeNode root)
        {
            this.root = root;
        }
    }

    /// <summary>
    /// Shift-Reduce Parser for LR(1)
    /// </summary>
    public class ShiftReduceParser
    {
        Dictionary<string, int> symbol_name_index = new Dictionary<string, int>();
        List<string> symbol_index_name = new List<string>();
        Stack<int> state_stack = new Stack<int>();
        Stack<ParsingTree.ParsingTreeNode> treenode_stack = new Stack<ParsingTree.ParsingTreeNode>();

        // 3       1      2       0
        // Accept? Shift? Reduce? Error?
        int[][] jump_table;
        int[][] goto_table;
        int[][] production;
        int[] group_table;

        public ShiftReduceParser(Dictionary<string, int> symbol_table, int[][] jump_table, int[][] goto_table, int[] group_table, int[][] production)
        {
            symbol_name_index = symbol_table;
            this.jump_table = jump_table;
            this.goto_table = goto_table;
            this.production = production;
            this.group_table = group_table;
            var l = symbol_table.ToList().Select(x => new Tuple<int, string>(x.Value, x.Key)).ToList();
            l.Sort();
            l.ForEach(x => symbol_index_name.Add(x.Item2));
        }

        bool latest_error;
        bool latest_reduce;
        public bool Accept() => state_stack.Count == 0;
        public bool Error() => latest_error;
        public bool Reduce() => latest_reduce;

        public void Clear()
        {
            latest_error = latest_reduce = false;
            state_stack.Clear();
            treenode_stack.Clear();
        }

        public ParsingTree Tree => new ParsingTree(treenode_stack.Peek());

        public string Stack() => string.Join(" ", new Stack<int>(state_stack));

        public void Insert(string token_name, string contents) => Insert(symbol_name_index[token_name], contents);
        public void Insert(int index, string contents)
        {
            if (state_stack.Count == 0)
            {
                state_stack.Push(0);
                latest_error = false;
            }
            latest_reduce = false;

            switch (jump_table[state_stack.Peek()][index])
            {
                case 0:
                    // Panic mode
                    state_stack.Clear();
                    treenode_stack.Clear();
                    latest_error = true;
                    break;

                case 1:
                    // Shift
                    state_stack.Push(goto_table[state_stack.Peek()][index]);
                    treenode_stack.Push(ParsingTree.ParsingTreeNode.NewNode(symbol_index_name[index], contents));
                    break;

                case 2:
                    // Reduce
                    reduce(index);
                    latest_reduce = true;
                    break;

                case 3:
                    // Nothing
                    break;
            }
        }

        public ParsingTree.ParsingTreeNode LatestReduce() => treenode_stack.Peek();
        private void reduce(int index)
        {
            var reduce_production = goto_table[state_stack.Peek()][index];
            var reduce_treenodes = new List<ParsingTree.ParsingTreeNode>();

            // Reduce Stack
            for (int i = 0; i < production[reduce_production].Length; i++)
            {
                state_stack.Pop();
                reduce_treenodes.Insert(0, treenode_stack.Pop());
            }

            state_stack.Push(goto_table[state_stack.Peek()][group_table[reduce_production]]);

            var reduction_parent = ParsingTree.ParsingTreeNode.NewNode(symbol_index_name[group_table[reduce_production]]);
            reduction_parent.ProductionRuleIndex = reduce_production - 1;
            reduce_treenodes.ForEach(x => x.Parent = reduction_parent);
            reduction_parent.Contents = string.Join("", reduce_treenodes.Select(x => x.Contents));
            reduction_parent.Childs = reduce_treenodes;
            treenode_stack.Push(reduction_parent);
        }
    }
}