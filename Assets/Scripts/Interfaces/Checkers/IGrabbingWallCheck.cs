namespace Interfaces.Checkers
{
    public interface IGrabbingWallCheck
    {
        public bool GrabbingWallLeft { get; set; }
        public bool GrabbingWallRight { get; set; }
        public bool GrabbingWall { get; set; }
        
        public bool Sliding { get; set; }
        public bool Falling { get; set; }
        public bool InAir { get; set; }
        
    }
}