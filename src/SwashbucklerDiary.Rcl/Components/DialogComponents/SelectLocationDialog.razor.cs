using DocumentFormat.OpenXml.Spreadsheet;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class SelectLocationDialog : DialogComponentBase
    {
        private bool showAdd;

        private bool showSearch;

        private string? _searchText;

        private StringNumber? internalValue;

        private List<StringNumber> internalValues = [];

        private List<LocationModel> internalItems = [];

        private readonly List<LocationModel> defaultItems = [];

        [Inject]
        ILocationService LocationService { get; set; } = default!;

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public List<string> Values { get; set; } = [];

        [Parameter]
        public EventCallback<List<string>> ValuesChanged { get; set; }

        [Parameter]
        public List<LocationModel> Items { get; set; } = [];

        [Parameter]
        public EventCallback<List<LocationModel>> ItemsChanged { get; set; }

        [Parameter]
        public bool Multiple { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Items = defaultItems;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                if (Items == defaultItems)
                {
                    Items = await LocationService.QueryAsync();
                    UpdateInternalItems(_searchText);
                    StateHasChanged();
                }
            }
        }

        private void BeforeShowContent()
        {
            _searchText = string.Empty;
            internalValue = Value;
            internalValues = [.. Values];
            internalItems = Items;
        }

        private async Task SaveAdd(string name)
        {
            showAdd = false;
            StateHasChanged();

            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            if (Items.Any(it => it.Name == name))
            {
                await AlertService.WarningAsync(I18n.T("Location already exists"), I18n.T("Do not add again"));
                return;
            }

            LocationModel location = new(name);
            var flag = await LocationService.AddAsync(location);
            if (!flag)
            {
                await AlertService.ErrorAsync(I18n.T("Add failed"));
                return;
            }

            Items.Insert(0, location);
            if (!Multiple)
            {
                internalValue = location.Name;
            }
            else
            {
                internalValues.Add(location.Name);
            }

            UpdateInternalItems(_searchText);
            StateHasChanged();
        }

        private async Task HandleOnOK()
        {
            await InternalVisibleChanged(false);

            if (!Multiple)
            {
                Value = internalValue?.ToString();
                if (ValueChanged.HasDelegate)
                {
                    await ValueChanged.InvokeAsync(Value);
                }
            }
            else
            {
                Values = internalValues.Select(it => it.ToString()).ToList();
                if (ValuesChanged.HasDelegate)
                {
                    await ValuesChanged.InvokeAsync(Values);
                }
            }
        }

        private void UpdateInternalItems(string? searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                internalItems = Items;
            }
            else
            {
                internalItems = Items.Where(it => !string.IsNullOrEmpty(it.Name) && (it.Name.Contains(searchText) || it.Name == internalValue)).ToList();
            }
        }
    }
}
