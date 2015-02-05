using System;
using InsideModel.Models;

namespace InsideReporting.Models
{
    public class LabelViewModel
    {
        public LabelViewModel(Label label)
        {
            this.Id = label.Id;
            this.Name = label.Name;
        }
        

        public int Id { get; set; }
        public String Name { get; set; }
    }
}