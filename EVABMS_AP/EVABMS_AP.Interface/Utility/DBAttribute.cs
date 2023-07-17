using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVABMS_AP.Interface
{
    /// <summary>The custome attribute.</summary>
    public class DbTableAttribute : Attribute
    {
        /// <summary>The _alias.</summary>
        private readonly string _alias;

        /// <summary>Gets the alias.</summary>
        public string Alias
        {
            get
            {
                return this._alias;
            }
        }

        /// <summary>Initializes a new instance of the <see cref="AliasAttribute"/> class.</summary>
        /// <param name="alias">The alias.</param>
        public DbTableAttribute(string alias)
        {
            this._alias = alias;
        }
    }
    /// <summary>The custome attribute.</summary>
    public class DbColumnAttribute : Attribute
    {
        /// <summary>The _alias.</summary>
        private readonly string _alias;

        /// <summary>Gets the alias.</summary>
        public string Alias
        {
            get
            {
                return this._alias;
            }
        }

        /// <summary>Initializes a new instance of the <see cref="AliasAttribute"/> class.</summary>
        /// <param name="alias">The alias.</param>
        public DbColumnAttribute(string alias)
        {
            this._alias = alias;
        }
    }
}
