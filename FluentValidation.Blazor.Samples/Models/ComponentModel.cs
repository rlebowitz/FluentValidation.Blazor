using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluentValidation.Blazor.Samples.Models
{
    public class ComponentModel
    {
        public Student Student { get; set; } = new Student();
        public string Teacher { get; set; }
    }
}
