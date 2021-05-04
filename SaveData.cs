using Microsoft.Xna.Framework;


namespace Issho
{
    public struct SaveData
    {
        public string LevelName;
        public Vector2 LevelPosition;

        public SaveData(string levelName, Vector2 levelPosition)
        {
            LevelName = levelName;
            LevelPosition = levelPosition;
        }

        public static bool operator ==(SaveData s1, SaveData s2) {
            return s1.Equals(s2);
        }

        public static bool operator !=(SaveData s1, SaveData s2) {
            return !s1.Equals(s2);
        }
    }
}
