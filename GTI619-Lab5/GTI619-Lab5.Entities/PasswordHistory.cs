//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GTI619_Lab5.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class PasswordHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
    
        public virtual User User { get; set; }
    }
}