// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameEditor.xaml.cs" company="OCTGN">
//   GNU Stuff
// </copyright>
// <summary>
//   Interaction logic for GameEditor.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Octgn.Play.Dialogs
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Xml;

    using AvalonDock.Layout;

    using ICSharpCode.AvalonEdit;
    using ICSharpCode.AvalonEdit.Highlighting;
    using ICSharpCode.AvalonEdit.Highlighting.Xshd;

    /// <summary>
    /// Interaction logic for GameEditor.xaml
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public partial class GameEditor : Window
    {
        /// <summary>
        /// The open documents.
        /// </summary>
        private Dictionary<string, LayoutDocument> documents;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameEditor"/> class.
        /// </summary>
        public GameEditor()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the current game.
        /// </summary>
        public Game CurrentGame { get; private set; }

        /// <summary>
        /// Load game and edit.
        /// </summary>
        /// <param name="game">
        /// The game.
        /// </param>
        public void LoadGame(Game game)
        {
            if (game == null || (this.CurrentGame != null && game.Definition.Id == this.CurrentGame.Definition.Id))
            {
                return;
            }

            this.CurrentGame = game;
            this.documents = new Dictionary<string, LayoutDocument>();

            DocumentPaneMain.Children.Clear();

            foreach (var s in game.Definition.Scripts)
            {
                if (!this.documents.ContainsKey(s.FileName))
                {
                    this.documents.Add(s.FileName, new LayoutDocument());
                }

                var te = new TextEditor();
                this.documents[s.FileName].Content = te;
                this.documents[s.FileName].CanClose = false;
                this.documents[s.FileName].Title = s.FileName;
                te.Text = s.Python;
                var psyntax = Encoding.ASCII.GetString(Properties.Resources.PythonSyntax);
                te.SyntaxHighlighting =
                    HighlightingLoader.Load(
                        new XmlTextReader(new StringReader(psyntax)),
                        HighlightingManager.Instance);

                DocumentPaneMain.Children.Add(this.documents[s.FileName]);
            }
        }
    }
}
