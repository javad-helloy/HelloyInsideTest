using System.Collections.Generic;
using InsideReporting.Models.Layout;

namespace InsideReporting.Models
{
    public class LabelsViewModel: LoggedInViewModel
    {
        public LabelsViewModel()
        {
            this.Labels = new List<LabelViewModel>();
        }
        public IList<LabelViewModel> Labels { get; set; }
    }
}