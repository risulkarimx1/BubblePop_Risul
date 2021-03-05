namespace Assets.Code.LevelGeneration
{
    public class LevelDataContext
    {
        private readonly LevelDataContainer _levelDataContainer;
        public int SelectedLevel { get; set; }

        public LevelDataContext(LevelDataContainer levelDataContainer)
        {
            _levelDataContainer = levelDataContainer;
        }

        public string GetSelectedLevelData()
        {
            return _levelDataContainer.GetLevelData(SelectedLevel);
        }

        public string GetSelectedLevelStrikerData()
        {
            return _levelDataContainer.GetStrikerData(SelectedLevel);
        }
    }
}