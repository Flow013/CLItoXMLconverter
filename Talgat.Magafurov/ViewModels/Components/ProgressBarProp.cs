namespace Talgat.Magafurov.ViewModels.Components
{
    internal class ProgressBarProp : BaseViewModel
    {
        private int _progressBarValue = 0;
        private int _progressBarMax = 100;
        private int _progressBarMin = 0;

        public int ProgressBarValue
        {
            get => _progressBarValue;
            set
            {
                _progressBarValue = value;
                OnPropertyChanged(nameof(ProgressBarValue));
            }
        }

        public int ProgressBarMax
        {
            get => _progressBarMax;
            set
            {
                _progressBarMax = value;
                OnPropertyChanged(nameof(ProgressBarMax));
            }
        }

        public int ProgressBarMin
        {
            get => _progressBarMin;
            set
            {
                _progressBarMin = value;
                OnPropertyChanged(nameof(ProgressBarMin));
            }
        }
    }
}