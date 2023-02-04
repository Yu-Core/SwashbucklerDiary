using BlazorComponent;
using BlazorComponent.I18n;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using NoDecentDiary.IServices;
using NoDecentDiary.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Components
{
    public partial class SelectTags : MyComponentBase, IDisposable
    {
        private bool _value;
        private bool _showAddTag;
        private string? AddTagName;
        private List<StringNumber> SelectedTagIndices = new ();
        private List<TagModel> Tags = new();

        [Inject]
        public ITagService TagService { get; set; } = default!;

        [Parameter]
        public bool Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    InitSelectedTags(value);
                }
            }
        }
        [Parameter]
        public EventCallback<bool> ValueChanged { get; set; }
        [Parameter]
        public List<TagModel> Values { get; set; } = new List<TagModel>();
        [Parameter]
        public EventCallback<List<TagModel>> ValuesChanged { get; set; }
        [Parameter]
        public EventCallback OnSave { get; set; }

        public void Dispose()
        {
            if (ShowAddTag)
            {
                NavigateService.Action -= CloseAddTag;
            }
            GC.SuppressFinalize(this);
        }

        protected override async Task OnInitializedAsync()
        {
            Tags = await TagService!.QueryAsync();
        }

        protected virtual async Task HandleOnCancel(MouseEventArgs _)
        {
            await InternalValueChanged(false);
        }

        protected virtual async Task HandleOnSave(MouseEventArgs _)
        {
            var TagModels = new List<TagModel>();
            foreach (var item in SelectedTagIndices)
            {
                var TagModel = Tags[item.ToInt32()];
                TagModels.Add(TagModel);
            }
            Values = TagModels;

            if (ValuesChanged.HasDelegate)
            {
                await ValuesChanged.InvokeAsync(TagModels);
            }

            await OnSave.InvokeAsync();
        }

        private async Task InternalValueChanged(bool value)
        {
            Value = value;

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(value);
            }
        }

        private bool ShowAddTag
        {
            get => _showAddTag;
            set
            {
                SetShowAddTag(value);
            }
        }

        private async void InitSelectedTags(bool value)
        {
            if (value)
            {
                Tags = await TagService!.QueryAsync();
                SelectedTagIndices.Clear();
                foreach (var item in Values)
                {
                    int index = Tags.FindIndex(it => it.Id == item.Id); ;
                    if (index > -1)
                    {
                        SelectedTagIndices.Add(index);
                    }
                }
            }
        }

        private async void SaveAddTag()
        {
            ShowAddTag = false;
            if (string.IsNullOrWhiteSpace(AddTagName))
            {
                return;
            }

            if (Tags.Any(it => it.Name == AddTagName))
            {
                await PopupService.ToastAsync(it =>
                {
                    it.Type = AlertTypes.Warning;
                    it.Title = I18n!.T("Tag.Repeat.Title");
                    it.Content = I18n!.T("Tag.Repeat.Content");
                });
                return;
            }

            TagModel tagModel = new ()
            {
                Name = AddTagName
            };
            bool flag = await TagService!.AddAsync(tagModel);
            if (!flag)
            {
                await PopupService.ToastAsync(it =>
                {
                    it.Type = AlertTypes.Error;
                    it.Title = I18n!.T("Share.AddFail");
                });
                return;
            }

            await PopupService.ToastAsync(it =>
            {
                it.Type = AlertTypes.Success;
                it.Title = I18n!.T("Share.AddSuccess");
            });
            tagModel.Id = await TagService!.GetLastInsertRowId();
            Tags.Add(tagModel);
            StateHasChanged();
        }

        private void SetShowAddTag(bool value)
        {
            if (_showAddTag != value)
            {
                _showAddTag = value;
                if (value)
                {
                    NavigateService.Action += CloseAddTag;
                }
                else
                {
                    NavigateService.Action -= CloseAddTag;
                }
            }
        }

        private void CloseAddTag()
        {
            ShowAddTag = false;
            StateHasChanged();
        }
    }
}
