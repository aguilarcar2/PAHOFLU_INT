//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Paho
{
    using System;
    using System.Collections.Generic;
    
    public partial class Neighborhood
    {
        public Neighborhood()
        {
            this.CaseGEOs = new HashSet<CaseGEO>();
        }
    
        public int ID { get; set; }
        public int StateID { get; set; }
        public int Code { get; set; }
    
        public virtual ICollection<CaseGEO> CaseGEOs { get; set; }
        public virtual State State { get; set; }
    }
}
