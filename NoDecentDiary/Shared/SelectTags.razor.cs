using BlazorComponent;
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

namespace NoDecentDiary.Shared
{
    public partial class SelectTags : IDisposable
    {
        [Inject]
        public ITagService? TagService { get; set; }
        [Inject]
        public IPopupService? PopupService { get; set; }
        [Inject]
        public INavigateService? NavigateService { get; set; }
        [Parameter]
        public bool Value
        {
            get => _value;
            set
            {
                if(_value != value)
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

        private bool _value;
        private bool _showAddTag;
        private bool ShowAddTag
        {
            get => _showAddTag;
            set
            {
                SetShowAddTag(value);
            }
        }
        private string? AddTagName;
        private List<StringNumber> SelectedTagIndices { get; set; } = new List<StringNumber>();
        private List<TagModel> Tags { get; set; } = new List<TagModel>();

        protected override async Task OnInitializedAsync()
        {
            Tags = await TagService!.QueryAsync();
        }

        protected virtual async Task HandleOnCancel(MouseEventArgs _)
        {
            await InternalValueChanged(false);
        }

        private async Task InternalValueChanged(bool value)
        {
            Value = value;

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(value);
            }
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
        private async void HandleOnSaveAddTag()
        {
            ShowAddTag = false;
            if(string.IsNullOrWhiteSpace(AddTagName))
            {
                return;
            }

            if(Tags.Any(it=>it.Name == AddTagName))
            {
                await PopupService!.AlertAsync("标签已存在", AlertTypes.Warning);
                return;
            }

            TagModel tagModel = new TagModel()
            {
                Name = AddTagName
            };
            bool flag = await TagService!.AddAsync(tagModel);
            if (!flag)
            {
                await PopupService!.AlertAsync("添加失败",AlertTypes.Error);
                return;
            }

            await PopupService!.AlertAsync("添加成功", AlertTypes.Success);
            tagModel.Id = await TagService!.GetLastInsertRowId();
            Tags.Add(tagModel);
            this.StateHasChanged();
        }
        private void SetShowAddTag(bool value)
        {
            if (_showAddTag != value)
            {
                _showAddTag = value;
                if (value)
                {
                    NavigateService!.Action += CloseAddTag;
                }
                else
                {
                    NavigateService!.Action -= CloseAddTag;
                }
            }
        }
        private void CloseAddTag()
        {
            ShowAddTag = false;
            StateHasChanged();
        }
        public void Dispose()
        {
            if (ShowAddTag)
            {
                NavigateService!.Action -= CloseAddTag;
            }
            GC.SuppressFinalize(this);
        }
    }
}
