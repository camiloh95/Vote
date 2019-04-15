﻿namespace Vote.UIForms.ViewModels
{
    using Common.Models;
    using Common.Services;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Xamarin.Forms;

    public class VoteEventsViewModel : BaseViewModel
    {
        private readonly ApiService apiService;
        private List<VoteEvent> myVoteEvents;
        private ObservableCollection<VoteEventItemViewModel> voteEvents;
        private bool isRefreshing;

        public ObservableCollection<VoteEventItemViewModel> VoteEvents
        {
            get => this.voteEvents;
            set => this.SetValue(ref this.voteEvents, value);
        }

        public bool IsRefreshing
        {
            get => this.isRefreshing;
            set => this.SetValue(ref this.isRefreshing, value);
        }

        public VoteEventsViewModel()
        {
            this.apiService = new ApiService();
            this.LoadVoteEvents();
        }

        private async void LoadVoteEvents()
        {
            this.IsRefreshing = true;

            var url = Application.Current.Resources["UrlAPI"].ToString();
            var response = await this.apiService.GetListAsync<VoteEvent>(
                url,
                "/api",
                "/VoteEvents",
                "bearer",
                MainViewModel.GetInstance().Token.Token);

            this.IsRefreshing = false;

            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Message,
                    "Accept");
                return;
            }

            this.myVoteEvents = (List<VoteEvent>)response.Result;
            this.RefresProductsList();
        }

        public void AddProductToList(VoteEvent product)
        {
            this.myVoteEvents.Add(product);
            this.RefresProductsList();
        }

        public void UpdateProductInList(VoteEvent product)
        {
            var previousProduct = this.myVoteEvents.Where(p => p.Id == product.Id).FirstOrDefault();
            if (previousProduct != null)
            {
                this.myVoteEvents.Remove(previousProduct);
            }

            this.myVoteEvents.Add(product);
            this.RefresProductsList();
        }

        public void DeleteProductInList(int productId)
        {
            var previousProduct = this.myVoteEvents.Where(p => p.Id == productId).FirstOrDefault();
            if (previousProduct != null)
            {
                this.myVoteEvents.Remove(previousProduct);
            }

            this.RefresProductsList();
        }

        private void RefresProductsList()
        {
            this.myVoteEvents = new ObservableCollection<ViewVoteEventViewModel>(
                this.myVoteEvents.Select(p => new ViewVoteEventViewModel
                {
                })
            .OrderBy(p => p.Name)
            .ToList());
        }
    }
}
