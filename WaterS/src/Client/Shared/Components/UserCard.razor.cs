using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using WaterS.Client.Extensions;

namespace WaterS.Client.Shared.Components
{
    public partial class UserCard
    {
        [Parameter] public string Class { get; set; }
        private string FirstName { get; set; }
        private string SecondName { get; set; }
        private string Email { get; set; }
        private string Phone { get; set; }
        private bool IsCustomer { get; set; } = false;

        private char FirstLetterOfName { get; set; }

        [Parameter]
        public string ImageDataUrl { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;

            this.Email = user.GetEmail().Replace(".com", string.Empty);
            this.FirstName = user.GetFirstName();
            this.SecondName = user.GetLastName();
            this.Phone = user.GetPhoneNumber();
            if (this.FirstName.Length > 0)
            {
                FirstLetterOfName = FirstName[0];
            }
            var UserId = user.GetUserId();
            try
            {
                var imageResponse = await _accountManager.GetProfilePictureAsync(UserId);
                if (imageResponse.Succeeded)
                {
                    ImageDataUrl = imageResponse.Data;
                }
            }
            catch 
            {
                ImageDataUrl = null;

                return;
            }
           
        }
    }
}