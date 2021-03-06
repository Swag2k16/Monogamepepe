using PepesComing.Solvers;

namespace PepesComing
{

    public class WallFollower : SolverMouse {

        public WallFollower(ref World world) : base(ref world) {
            FaceEmpty();
        }

        public override void Step() {
            if (!Done()) {
                var ahead = LookAhead(ref world);
                var left = LookLeft(ref world);

                if (!ahead.Type.Blocked() && left.Type.Blocked()) {
                    Move();
                } else if (left.Type == CellType.FLOOR) {
                    TurnLeft();
                    Move();
                } else {
                    TurnRight();
                }
            }
        }
    }
}
