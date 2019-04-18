﻿namespace Vote.UIForms.ViewModels
{
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Common.Models;
    using UIForms.Views;

    public class VoteEventItemViewModel : VoteEvent
    {
        public ICommand SelectVoteEventCommand => new RelayCommand(this.SelectVoteEvent);

        private async void SelectVoteEvent()
        {
            MainViewModel.GetInstance().Candidates = new CandidatesViewModel((VoteEvent)this);
            await App.Navigator.PushAsync(new CandidatesPage());
        }
    }
}