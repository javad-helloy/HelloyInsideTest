using System.Collections.Generic;

namespace InsideModel.Models
{
    public partial class Label
    {
        public Label()
        {
            this.Clients = new List<Client>();    
        }

        public int Id { get; set; }
        public string Name { get; set; }
        
        public virtual ICollection<Client> Clients { get; set; }
    }
}
