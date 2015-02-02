using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportMayhem.Controller;
using TransportMayhem.View;

namespace TransportMayhem.Model.MovingObjects
{
    class Train : MovingObject, IUpdateable
    {
        private float speed;
        private float radialSpeed;
        public override View.MovingRenderers.IMovingRenderer Renderer { get { return Renderers.TrainRenderer; } }
        private Rotation _direction;
        private bool updating = false;
        /// <summary>
        /// Where did the train come from
        /// </summary>
        public Rotation From { get; private set; }
        public Rotation Direction {
            get { return _direction; }
            private set {
                From = RailUtils.OppositeRotation(_direction);
                Rotate = RailUtils.RotationToAngle(_direction);
                if (value != _direction)
                    _direction = value;
                else
                    Rotate = RailUtils.RotationToAngle(value);
            }
        }
        private float _rotate;
        public float Rotate
        {
            get { return _rotate; }
            private set { _rotate = value; }
        }

        public Train(float x, float y, Rotation rotation, float speed = 0.05f)
            : base(x, y)
        {
            Direction = rotation;
            From = RailUtils.OppositeRotation(rotation);
            Rotate = RailUtils.RotationToAngle(Direction);
            this.speed = speed;
            this.radialSpeed = 90 * speed;
        }

        public Train(int x, int y, Rotation rotation) : this(x + 0.5f, y + 0.5f, rotation) { }

        Random tempRandom = new Random();
        private bool GetNewDirection()
        {
            Rotation[] rotations = TrainNetworkRegistry.GetDirections(this);
            if (rotations.Length == 0)
             {
                 RotateTrain();
                TMConsole.Log("Train turned due to not being able to move further.", this);
                return false;
            }
            else
            {
                Direction = rotations[tempRandom.Next(rotations.Length)];
                return true;
            }
        }

        public void QuickUpdate()
        {
            if (updating) return;
            updating = true;
            PointF currentLocation = LocationF;
            Point currentGrid = Location;
            CalculateNewPosition();
            if (Location != currentGrid)
                if(!GetNewDirection())
                    LocationF = currentLocation;
            updating = false;
        }

        private void CalculateRotation(Point rotationPoint, bool clockwise)
        {
            const float radius = 0.5f;
            Rotate += clockwise ? radialSpeed : -radialSpeed;
            float rotation = Rotate + (clockwise ? -90 : 90);
            float cos = (float)Math.Cos(Math.PI * rotation / 180);
            float sin = (float)Math.Sin(Math.PI * rotation / 180);
            X = rotationPoint.X + radius * cos ;
            Y = rotationPoint.Y + radius * sin;
        }

        
        private void CalculateNewPosition()
        {
            switch (Direction)
            {
                case Rotation.Top:
                    switch (From)
	                {
                        case Rotation.Right:
                            CalculateRotation(new Point(GridX + 1, GridY), true);
                            return;
                        case Rotation.Bottom:
                            Y -= speed;
                            return;
                        case Rotation.Left:
                            CalculateRotation(Location, false);
                            return;
                        default:
                            return;
	                }
                case Rotation.Right:
                    switch (From)
                    {
                        case Rotation.Left:
                            X += speed;
                            return;
                        case Rotation.Top:
                            CalculateRotation(new Point(GridX + 1, GridY), false);
                            return;
                        case Rotation.Bottom:
                            CalculateRotation(new Point(GridX + 1, GridY + 1), true);
                            return;
                        default:
                            return;
                    }
                case Rotation.Bottom:
                    switch (From)
                    {
                        case Rotation.Top:
                            Y += speed;
                            return;
                        case Rotation.Left:
                            CalculateRotation(new Point(GridX, GridY + 1), true);
                            return;
                        case Rotation.Right:
                            CalculateRotation(new Point(GridX + 1, GridY + 1), false);
                            return;
                        default:
                            return;
                    }
                case Rotation.Left:
                    switch (From)
                    {
                        case Rotation.Right:
                            X -= speed;
                            return;
                        case Rotation.Top:
                            CalculateRotation(Location, true);
                            return;
                        case Rotation.Bottom:
                            CalculateRotation(new Point(GridX, GridY + 1), false);
                            return;
                        default:
                            return;
                    }
            }
        }

        public void SlowUpdate()
        {

        }

        private void RotateTrain()
        {
            Rotation temp = From;
            From = _direction;
            _direction = temp;
            Rotate = (Rotate + 180) % 360;
        }


        public override string ToString()
        {
            return String.Format("Train moving from {0} to {1} ({2}/{3}/{4}°)", From, Direction, X.ToString("F2"), Y.ToString("F2"), Rotate);
        }
    }
}
