using System.Collections;
using macaroni_dev.Models;
using Syncfusion.Maui.Inputs;

namespace macaroni_dev.Messengers;

/// <summary>
/// Represents a custom filtering behavior for a ComboBox that filters Skill items by Name.
/// </summary>
public class SkillsFilteringBehavior : IComboBoxFilterBehavior
{
    /// <summary>
    /// Returns a filtered and ordered list of skills where Name contains the filter text,
    /// with priority given to names that start with the text.
    /// </summary>
    public async Task<object?> GetMatchingIndexes(SfComboBox source, ComboBoxFilterInfo filterInfo)
    {
        IEnumerable itemssource = source.ItemsSource as IEnumerable;

        if (itemssource == null || string.IsNullOrWhiteSpace(filterInfo.Text))
            return await Task.FromResult(itemssource);

        var text = filterInfo.Text;

        
        var filteredItems = (from Skill skill in itemssource
            where skill.Name.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0
            orderby skill.Name.StartsWith(text, StringComparison.CurrentCultureIgnoreCase) ? 0 : 1,
                skill.Name
            select skill);

        return await Task.FromResult(filteredItems);
    }
}