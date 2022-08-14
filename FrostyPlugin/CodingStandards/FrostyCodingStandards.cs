namespace Frosty.Core.CodingStandards
{
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
        
        // private member variables should be camelCase prefixed with "m_"
        private string m_inputName;
        // bool should always be prefixed with a modal verb, and no 'b' prefix
        private bool m_isOpen;
        private bool m_shouldOpenMenu;

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
    }
}