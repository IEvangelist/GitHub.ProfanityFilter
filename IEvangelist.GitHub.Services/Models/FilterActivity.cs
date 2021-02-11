using IEvangelist.GitHub.Services.Enums;
using Microsoft.Azure.CosmosRepository;
using System;

namespace IEvangelist.GitHub.Services.Models
{
    public class FilterActivity : Item
    {
        public string MutationOrNodeId { get; set; }
        
        public ActivityType ActivityType { get; set; }

        public DateTime WorkedOn { get; set; }

        public bool WasProfane { get; set; }

        public string OriginalTitleText { get; set; }

        public string ModifiedTitleText { get; set; }

        public string OriginalBodyText { get; set; }

        public string ModifiedBodyText { get; set; }
    }
}