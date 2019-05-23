﻿namespace Vote.Common.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Helpers;
    using Interfaces;
    using Models;
    using MvvmCross.Commands;
    using MvvmCross.Navigation;
    using MvvmCross.ViewModels;
    using Newtonsoft.Json;
    using Services;
    
    public class CandidatesViewModel : MvxViewModel<NavigationArgs>
    {
        private readonly IApiService apiService;
        private readonly IDialogService dialogService;
        private readonly IMvxNavigationService navigationService;
        private List<Candidate> candidates;
        private MvxCommand<Candidate> itemClickCommand;
        private bool isLoading;

        public CandidatesViewModel(
            IApiService apiService,
            IDialogService dialogService,
            IMvxNavigationService navigationService)
        {
            this.apiService = apiService;
            this.dialogService = dialogService;
            this.navigationService = navigationService;
            this.IsLoading = false;
        }
        public SaveVoteRequest SaveVoteRequest { get; set; }

        public ICommand ItemClickCommand
        {
            get
            {
                this.itemClickCommand = new MvxCommand<Candidate>(this.OnItemClickCommand);
                return itemClickCommand;
            }
        }

        public bool IsLoading
        {
            get => this.isLoading;
            set => this.SetProperty(ref this.isLoading, value);
        }

        public List<Candidate> Candidates
        {
            get => this.candidates;
            set => this.SetProperty(ref this.candidates, value);
        }

        private async void OnItemClickCommand(Candidate candidate)
        {
            var user = await this.GetUserId();
            this.SaveVoteRequest = new SaveVoteRequest
            {
                CandidateId = candidate.Id,
                UserId = user.Id
            };
            this.Vote();
        }

        private async Task<User> GetUserId()
        {
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            var response = await this.apiService.GetUserByEmailAsync(
                "https://camilovoting.azurewebsites.net",
                "/api",
                "/Account/GetUserByEmail",
                Settings.UserEmail,
                "bearer",
                token.Token);

            if (!response.IsSuccess)
            {
                this.IsLoading = false;
                this.dialogService.Alert("Error", "Error en la consulta", "Accept");
                return null;
            }
            
            return (User)response.Result;
        }

        private async void Vote()
        {
            this.IsLoading = true;

            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            var response = await this.apiService.PostAsync(
                "https://camilovoting.azurewebsites.net",
                "/api",
                "/VoteEvents/SaveVote",
                this.SaveVoteRequest,
                "bearer",
                token.Token);

            this.IsLoading = false;

            if (!response.IsSuccess)
            {
                this.dialogService.Alert("Error", response.Message, "Accept");
                return;
            }

            await this.navigationService.Close(this);
        }
        
        public override void Prepare(NavigationArgs parameter)
        {
            this.candidates = parameter.VoteEvent.Candidates;
        }
    }
}