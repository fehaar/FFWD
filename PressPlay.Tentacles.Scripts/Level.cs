using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.Tentacles.Scripts
{
    [System.Serializable]
    public class Level
    {
        public string name;
        public string nameKey;
        public int id;
        public int batch;
        public int worldId;
        public int worldsIndex;

        public int themeId;
        public string themeName;

        public int levelsIndex;
        public string sceneName;
        public int numberOfLives = 3;
        public int bronzeScore = 0;
        public int silverScore = 0;
        public int goldScore = 0;
        public string hintscreen;

        public int nextLevel = -1;
        public int prevLevel = -1;

        public LevelType levelType = LevelType.desatGreen;
        public enum LevelType
        {
            veins,
            intestines,
            brain,
            desatGreen,
            petriDish
        }


        public Level(LevelImportDataStructure data)
        {
            name = data.level_name;
            id = int.Parse(data.level_id);
            worldId = int.Parse(data.world_id);
            sceneName = data.scene_name;
            batch = int.Parse(data.batch);

            //Debug.Log("data.theme_id :'"+data.theme_id+"'");

            if (data.theme_id != "")
            {
                themeId = int.Parse(data.theme_id);
            }
            themeName = data.theme_name;

            SetLevelTheme(themeName);

            if (data.bronze_score != "")
            {
                bronzeScore = int.Parse(data.bronze_score);
            }

            if (data.silver_score != "")
            {
                silverScore = int.Parse(data.silver_score);
            }

            if (data.gold_score != "")
            {
                goldScore = int.Parse(data.gold_score);
            }


        }

        public int CalculateNumberOfMedals(int score)
        {
            if (score >= goldScore)
            {
                return 3;
            }
            else if (score >= silverScore)
            {
                return 2;
            }
            else if (score >= bronzeScore)
            {
                return 1;
            }

            return 0;
        }

        private void SetLevelTheme(string theme)
        {
            switch (theme)
            {
                case "brains":
                    levelType = LevelType.brain;
                    break;

                case "veins":
                    levelType = LevelType.veins;
                    break;

                case "intestines":
                    levelType = LevelType.intestines;
                    break;

                case "desatGreen":
                    levelType = LevelType.desatGreen;
                    break;

                case "petriDish":
                    levelType = LevelType.petriDish;
                    break;

                default:
                    levelType = LevelType.desatGreen;
                    break;
            }
        }

        public static Level CreateDummyLevel(string _sceneName)
        {
            LevelImportDataStructure _dataStructure = new LevelImportDataStructure();
            _dataStructure.level_id = "-1";
            _dataStructure.batch = "-1";
            _dataStructure.world_id = "-1";
            _dataStructure.bronze_score = "0";
            _dataStructure.silver_score = "0";
            _dataStructure.gold_score = "0";
            _dataStructure.theme_id = "0";
            _dataStructure.theme_name = "";

            _dataStructure.scene_name = _sceneName;
            return new Level(_dataStructure);
        }
    }
}
