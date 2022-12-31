using BlazorComponent;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using NoDecentDiary.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Shared
{
    public partial class SelectTags
    {
        [Parameter]
        public bool Value
        {
            get => value;
            set
            {
                this.value = value;
                InitSelectedTags(value);
            }
        }
        [Parameter]
        public EventCallback<bool> ValueChanged { get; set; }
        [Parameter]
        public List<TagModel> Items { get; set; } = new List<TagModel>();
        [Parameter]
        public EventCallback<List<TagModel>> ItemsChanged { get; set; }

        private bool value;
        private List<StringNumber> SelectedTags { get; set; } = new List<StringNumber>();
        private List<TagModel> Tags { get; set; } = new List<TagModel>()
        {
            new TagModel(){Name="标签1"},
            new TagModel(){Name="标签2"},
            new TagModel(){Name="标签3"},
            new TagModel(){Name="标签4"},
            new TagModel(){Name="标签5"},
            new TagModel(){Name="标签6"},
        };


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
            await InternalItemsChanged();
        }

        private async Task InternalItemsChanged()
        {
            var TagModels = new List<TagModel>();
            foreach (var item in SelectedTags)
            {
                var TagModel = Tags[item.ToInt32()];
                TagModels.Add(TagModel);
            }
            Items = TagModels;

            if (ItemsChanged.HasDelegate)
            {
                await ItemsChanged.InvokeAsync(Items);
            }

            await InternalValueChanged(false);
        }

        private void InitSelectedTags(bool value)
        {
            if (value)
            {
                SelectedTags = new List<StringNumber>();
                foreach (var item in Items)
                {
                    int index = Tags.IndexOf(item);
                    if (index > -1)
                    {
                        SelectedTags.Add(index);
                    }
                }
            }
        }
    }
}
