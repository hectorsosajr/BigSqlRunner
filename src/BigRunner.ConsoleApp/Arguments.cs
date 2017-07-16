namespace BigRunner.ConsoleApp
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Arguments class
    /// </summary>
    public class Arguments
    {
        // Variables
        private readonly StringDictionary parameters;
        private readonly Hashtable intParameters = new Hashtable();
        private readonly int _index;

        // Constructor
        public Arguments(IEnumerable<string> args)
        {
            this.parameters = new StringDictionary();
            var spliter = new Regex(@"^-{1,2}|^/|=|:",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            var remover = new Regex(@"^['""]?(.*?)['""]?$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            string parameter = null;

            // Valid parameters forms:
            // {-,/,--}param{ ,=,:}((",')value(",'))
            // Examples: 
            // -param1 value1 --param2 /param3:"Test-:-work" 
            //   /param4=happy -param5 '--=nice=--'
            foreach (string txt in args)
            {
                // Look for new parameters (-,/ or --) and a
                // possible enclosed value (=,:)
                string[] parts = spliter.Split(txt, 3);

                switch (parts.Length)
                {
                    // Found a value (for the last parameter 
                    // found (space separator))
                    case 1:
                        if (parameter != null)
                        {
                            if (!this.parameters.ContainsKey(parameter))
                            {
                                parts[0] = remover.Replace(parts[0], "$1");

                                this.parameters.Add(parameter, parts[0]);
                                this.intParameters.Add(_index, parts[0]);

                                _index += 1;
                            }

                            parameter = null;
                        }

                        // else Error: no parameter waiting for a value (skipped)
                        break;

                    // Found just a parameter
                    case 2:
                        // The last parameter is still waiting. 
                        // With no value, set it to true.
                        if (parameter != null)
                        {
                            if (!this.parameters.ContainsKey(parameter))
                            {
                                this.parameters.Add(parameter, "true");
                                this.intParameters.Add(_index, parameter);

                                _index += 1;
                            }
                        }

                        parameter = parts[1];
                        break;

                    // Parameter with enclosed value
                    case 3:
                        // The last parameter is still waiting. 
                        // With no value, set it to true.
                        if (parameter != null)
                        {
                            if (!this.parameters.ContainsKey(parameter))
                            {
                                this.parameters.Add(parameter, "true");
                                this.intParameters.Add(_index, parameter);

                                _index += 1;
                            }
                        }

                        parameter = parts[1];

                        // Remove possible enclosing characters (",')
                        if (!this.parameters.ContainsKey(parameter))
                        {
                            parts[2] = remover.Replace(parts[2], "$1");
                            this.parameters.Add(parameter, parts[2]);
                            this.intParameters.Add(_index, parts[2]);

                            _index += 1;
                        }

                        parameter = null;
                        break;
                }
            }

            // In case a parameter is still waiting
            if (parameter != null)
            {
                if (this.parameters.ContainsKey(parameter)) return;
                this.parameters.Add(parameter, "true");
                this.intParameters.Add(this._index, parameter);

                this._index += 1;
            }
        }

        // Retrieve a parameter value if it exists 
        // (overriding C# indexer property)
        public string this[string param] => (this.parameters[param]);

        public string this[int index] => this.intParameters[index].ToString();

        public int Count => this.parameters.Count;
    }
}