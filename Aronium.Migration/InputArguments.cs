using System;
using System.Collections.Generic;

namespace Aronium.Migration
{
    public class InputArguments
    {
        #region - Constants -
        public const string DEFAULT_KEY_LEADING_PATTERN = "-"; 
        #endregion
        
        #region - Fields -

        private Dictionary<string, string> _parsedArguments = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly string _keyLeadingPattern;

        #endregion

        #region - Constructors -

        public InputArguments(string[] args)
            : this(args, null)
        {
        }

        public InputArguments(string[] args, string keyLeadingPattern)
        {
            _keyLeadingPattern = !string.IsNullOrEmpty(keyLeadingPattern) ? keyLeadingPattern : DEFAULT_KEY_LEADING_PATTERN;

            if (args != null && args.Length > 0)
                Parse(args);
        } 

        #endregion

        #region - Properties -

        /// <summary>
        /// Gets value for specified key.
        /// </summary>
        /// <param name="key">Key to find.</param>
        /// <returns>Key valud, if any, otherwise null.</returns>
        public string this[string key]
        {
            get { return GetValue(key); }
            set
            {
                if (key != null)
                    _parsedArguments[key] = value;
            }
        }
        
        /// <summary>
        /// Gets key leading pattern.
        /// </summary>
        public string KeyLeadingPattern
        {
            get { return _keyLeadingPattern; }
        }

        /// <summary>
        /// Gets parsed arguments as key value collection.
        /// </summary>
        public Dictionary<string, string> ArgumentsCollection
        {
            get { return _parsedArguments; }
        }

        #endregion

        #region - Public methods -

        /// <summary>
        /// Gets a value indicating whether arguments containts specified key.
        /// </summary>
        /// <param name="key">Key to find.</param>
        /// <returns>True if arguments contains key, otherwise false.</returns>
        public bool Contains(string key)
        {
            string adjustedKey;
            return ContainsKey(key, out adjustedKey);
        }

        /// <summary>
        /// Gets key without leading pattern.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>Trimmed key.</returns>
        public virtual string GetPeeledKey(string key)
        {
            return IsKey(key) ? key.Substring(_keyLeadingPattern.Length) : key;
        }

        /// <summary>
        /// Gets key with leading pattern prepended.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>Key with leading pattern prepended.</returns>
        public virtual string GetDecoratedKey(string key)
        {
            return !IsKey(key) ? (_keyLeadingPattern + key) : key;
        }

        /// <summary>
        /// Gets a value indicating whether argument is key.
        /// </summary>
        /// <param name="str">String argument.</param>
        /// <returns>True if argument is key, otherwise false.</returns>
        public virtual bool IsKey(string str)
        {
            return str.StartsWith(_keyLeadingPattern);
        }

        #endregion

        #region - Internal methods -

        protected virtual void Parse(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == null) continue;

                string key = null;
                string val = null;

                if (IsKey(args[i]))
                {
                    key = args[i];

                    if (i + 1 < args.Length && !IsKey(args[i + 1]))
                    {
                        val = args[i + 1];
                        i++;
                    }
                }
                else
                    val = args[i];

                // adjustment
                if (key == null)
                {
                    key = val;
                    val = null;
                }
                _parsedArguments[key] = val;
            }
        }

        protected virtual string GetValue(string key)
        {
            string adjustedKey;
            if (ContainsKey(key, out adjustedKey))
                return _parsedArguments[adjustedKey];

            return null;
        }

        protected virtual bool ContainsKey(string key, out string adjustedKey)
        {
            adjustedKey = key;

            if (_parsedArguments.ContainsKey(key))
                return true;

            if (IsKey(key))
            {
                string peeledKey = GetPeeledKey(key);
                if (_parsedArguments.ContainsKey(peeledKey))
                {
                    adjustedKey = peeledKey;
                    return true;
                }
                return false;
            }

            string decoratedKey = GetDecoratedKey(key);
            if (_parsedArguments.ContainsKey(decoratedKey))
            {
                adjustedKey = decoratedKey;
                return true;
            }
            return false;
        }

        #endregion
    }
}
