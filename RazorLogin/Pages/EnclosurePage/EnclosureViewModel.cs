namespace RazorLogin.ViewModels
{
    public class EnclosureViewModel
    {
        public int EnclosureId { get; set; }
        public string EnclosureName { get; set; }
        public string EnclosureDepartment { get; set; }
        public string OccupancyStatus { get; set; }
        public TimeOnly EnclosureOpenTime { get; set; }
        public TimeOnly EnclosureCloseTime { get; set; }
        public bool IsClosed { get; set; }
        public int? ZookeeperId { get; set; }  // Nullable to handle unassigned zookeepers
    }
}


