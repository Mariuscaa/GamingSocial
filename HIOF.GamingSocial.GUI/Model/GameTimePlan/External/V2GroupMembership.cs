namespace HIOF.GamingSocial.GameTimePlan.Model.External
{
    public class V2GroupMembership
    {
        public int GroupId { get; set; }
        public List<Guid> ProfileGuids { get; set; }

        public V2GroupMembership()
        {
        }
    }
}
