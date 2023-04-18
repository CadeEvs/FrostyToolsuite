
// namespaces should be file scoped
namespace Frosty.CodingStandards;

//
// Category Separator. First letter should be capitalized with no period at the end
//

// Small description for functions, classes, or member variables, should be capitalized with no period at the end

// small notes used within functions, shouldn't be capitalized or period at the end

/*
    * Longer description. First letter should be capitalized with period at the end.
    */

/*
    * Classes should be laid out like this,
    *
    * - Public member variables
    * - Private member variables
    * - Constructors
    * - Public Functions
    * - Private Functions
    *
    * Static and override functions/variables should always be at the top of each section.
    */
public class StandardClass
{
    // public member variables should be capitalized
    public string NodeName;

    // private static or constant member variables should be camelCase prefixed with "c_" or "s_"
    private const int c_magic = 123;
    private static int s_maxCount;
    
    // private or protected member variables should be camelCase prefixed with "m_"
    private string m_inputName;
    // bool should always be prefixed with a modal verb, and no 'b' prefix
    private bool m_isOpen;
    protected bool m_shouldOpenMenu;

    // if a class can be null it should be nullable
    private string? m_category;
    
    // class constructor parameter names should be prefixed with 'in' and camelCase
    public StandardClass(string inInputName, bool inIsOpen)
    {
        m_inputName = inInputName;
        m_isOpen = inIsOpen;
    }

    // Opens the context menu for the node
    public void OpenMenu()
    {
        // always surround execution logic with brackets on new lines
        if (!m_isOpen && m_shouldOpenMenu)
        {
            m_isOpen = true;
        }

        // Do NOT do this
        // if (!isOpen && shouldOpenMenu)
        //     isOpen = true;
    }

    public void DoSomething(string param)
    {
        // since classes should be nullable if they can be null this is unnecessary
        // if param can also be null change the type to string?
        // if (param is null)
        // {
        //     return;
        // }
        
        // using statements should be surrounded with brackets on new lines
        using (Stream stream = new MemoryStream())
        {
            byte[] array = new byte[2];
            stream.Write(array, 0, 2);
        }
    }
}

/*
    * For UI the MVVM Community Toolkit should be used.
    * ViewModels should inherit from ObservableObject.
*/
public class ViewModel : ObservableObject
{
    // properties that need to implement INotifyPropertyChanged should have the ObservableProperty attribute
    [ObservableProperty]
    private int m_property;
}