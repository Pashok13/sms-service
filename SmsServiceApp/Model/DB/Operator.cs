﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WebApp.Models
{
    /// <summary>
    /// Entity which represents cell phone provider
    /// </summary>
    public class Operator
    {
        public int Id { get; set; }
        public ICollection<Code> Codes { get; set; }
        public ICollection<Tariff> Tariffs { get; set; }

        /// <summary>
        /// Name of operator
        /// </summary>
        /// <value>
        /// Contains operator name, such as Vodafone
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Logo of operator
        /// </summary>
        /// <value>Contains logo as array of bites</value>
        /// <example>
        /// <code>
        /// byte[] imgData = null;
        /// using (var binReader = new BinaryReader(operatorItem.Logo.OpenReadStream()))
        /// {
        ///     imgData = binReader.ReadBytes((int)operatorItem.Logo.Length);
        /// }
        /// </code>
        /// </example>
        public byte[] Logo { get; set; }
    }
}
