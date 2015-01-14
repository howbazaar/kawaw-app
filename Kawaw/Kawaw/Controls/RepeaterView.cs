using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace Kawaw.Controls
{
    /// <summary>
    /// exception thrown when a template cannot
    /// be found for a supplied type
    /// </summary>
    public class NoDataTemplateMatchException : Exception
    {
        /// <summary>
        /// Hide any possible default constructor
        /// Redundant I know, but it costs nothing
        /// and communicates the design intent to
        /// other developers.
        /// </summary>
        private NoDataTemplateMatchException()
        {
        }

        /// <summary>
        /// Constructs the exception and passses a meaningful
        /// message to the base Exception
        /// </summary>
        /// <param name="tomatch">The type that a match was attempted for</param>
        /// <param name="candidates">All types examined during the match process</param>
        public NoDataTemplateMatchException(Type tomatch, List<Type> candidates) :
            base(string.Format("Could not find a template for type [{0}]", tomatch.Name))
        {
            AttemptedMatch = tomatch;
            TypesExamined = candidates;
            TypeNamesExamined = TypesExamined.Select(x => x.Name).ToList();
        }

        /// <summary>
        /// The type that a match was attempted for
        /// </summary>
        public Type AttemptedMatch { get; set; }

        /// <summary>
        /// A list of all types that were examined
        /// </summary>
        public List<Type> TypesExamined { get; set; }

        /// <summary>
        /// A List of the names of all examined types (Simple name only)
        /// </summary>
        public List<string> TypeNamesExamined { get; set; }
    }

    /// <summary>
    /// Thrown when datatemplate inflates to an object 
    /// that is neither a <see cref="Xamarin.Forms.View"/> object nor a
    /// <see cref="Xamarin.Forms.ViewCell"/> object
    /// </summary>
    public class InvalidVisualObjectException : Exception
    {
        /// <summary>
        /// Hide any possible default constructor
        /// Redundant I know, but it costs nothing
        /// and communicates the design intent to
        /// other developers.
        /// </summary>
        private InvalidVisualObjectException()
        {
        }

        /// <summary>
        /// Constructs the exception and passes a meaningful
        /// message to the base Exception
        /// </summary>
        /// <param name="inflatedtype">The actual type the datatemplate inflated to.</param>
        /// <param name="name">The calling methods name, uses [CallerMemberName]</param>
        public InvalidVisualObjectException(Type inflatedtype, [CallerMemberName] string name = null) :
            base(
            string.Format(
                "Invalid template inflated in {0}. Datatemplates must inflate to Xamarin.Forms.View(and subclasses) "
                + "or a Xamarin.Forms.ViewCell(or subclasses).\nActual Type received: [{1}]", name, inflatedtype.Name))
        {
        }

        /// <summary>
        /// The actual type the datatemplate inflated to.
        /// </summary>
        public Type InflatedType { get; set; }

        /// <summary>
        /// The MemberName the exception occured in.
        /// </summary>
        public string MemberName { get; set; }
    }

    /// <summary>
    /// Thrown when an invalid bindable object has been passed to a callback
    /// </summary>
    public class InvalidBindableException : Exception
    {

        /// <summary>
        /// Hide any possible default constructor
        /// Redundant I know, but it costs nothing
        /// and communicates the design intent to
        /// other developers.
        /// </summary>
        private InvalidBindableException()
        {
        }

        /// <summary>
        /// Constructs the exception and passes a meaningful
        /// message to the base Exception
        /// </summary>
        /// <param name="bindable">The bindable object that was passed</param>
        /// <param name="expected">The expected type</param>
        /// <param name="name">The calling methods name, uses [CallerMemberName]</param>
        public InvalidBindableException(BindableObject bindable, Type expected, [CallerMemberName] string name = null)
            : base(
                string.Format("Invalid bindable passed to {0} expected a {1} received a {2}", name, expected.Name,
                    bindable.GetType().Name))
        {
        }

        /// <summary>
        /// The bindable object that was passed
        /// </summary>
        public BindableObject IncorrectBindableObject { get; set; }

        /// <summary>
        /// The expected type of the bindable object
        /// </summary>
        public Type ExpectedType { get; set; }
    }

    /// <summary>
    /// Small utility class that takes
    /// gyuwon's idea to it's logical 
    /// conclusion.
    /// The code in the ItemsCollectionChanged methods
    /// rarely changes.  The only real change is projecting 
    /// from source type T to targeted type TSyncType which
    /// is then inserted into the target collection
    /// </summary>
    public class CollectionChangedHandle<TSyncType, T> : IDisposable
        where T : class
        where TSyncType : class
    {
        private readonly Func<T, TSyncType> _projector;
        private readonly Action<TSyncType, T, int> _postadd;
        private readonly Action<TSyncType> _cleanup;
        private readonly INotifyCollectionChanged _itemsSourceCollectionChangedImplementation;
        private readonly IEnumerable<T> _sourceCollection;
        private readonly IList<TSyncType> _target;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionChangedHandle{TSyncType,T}"/> class.
        /// </summary>
        /// <param name="target">The collection to be kept in sync with <see cref="source"/>source</param>
        /// <param name="source">The original collection</param>
        /// <param name="projector">A function that returns {TSyncType} for a {T}</param>
        /// <param name="postadd">A functino called right after insertion into the synced collection</param>
        /// <param name="cleanup">A function that performs any needed cleanup when {TSyncType} is removed from the <see cref="target"/></param>
        public CollectionChangedHandle(IList<TSyncType> target, IEnumerable<T> source, Func<T, TSyncType> projector,
            Action<TSyncType, T, int> postadd = null, Action<TSyncType> cleanup = null)
        {
            if (source == null) return;
            this._itemsSourceCollectionChangedImplementation = source as INotifyCollectionChanged;
            _sourceCollection = source;
            _target = target;
            _projector = projector;
            _postadd = postadd;
            _cleanup = cleanup;
            this.InitialPopulation();
            if (this._itemsSourceCollectionChangedImplementation == null) return;
            this._itemsSourceCollectionChangedImplementation.CollectionChanged += this.CollectionChanged;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this._itemsSourceCollectionChangedImplementation == null) return;
            this._itemsSourceCollectionChangedImplementation.CollectionChanged -= this.CollectionChanged;
        }

        /// <summary>Keeps <see cref="_target"/> in sync with <see cref="_sourceCollection"/>.</summary>
        /// <param name="sender">The sender, completely ignored.</param>
        /// <param name="args">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        /// Element created at 15/11/2014,2:57 PM by Charles
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Reset)
            {
                SafeClearTarget();
            }
            else
            {
                //Create a temp list to prevent multiple enumeration issues
                var tlist = new List<T>(_sourceCollection);

                if (args.OldItems != null)
                {
                    var syncitem = _target[args.OldStartingIndex];
                    if (syncitem != null && _cleanup != null) _cleanup(syncitem);
                    _target.RemoveAt(args.OldStartingIndex);
                }

                if (args.NewItems == null) return;
                foreach (var obj in args.NewItems)
                {
                    var item = obj as T;
                    if (item == null) continue;
                    var index = tlist.IndexOf(item);
                    var newsyncitem = this._projector(item);
                    this._target.Insert(index, newsyncitem);
                    if (_postadd != null) _postadd(newsyncitem, item, index);
                }
            }

        }

        /// <summary>Initials the population.</summary>
        /// Element created at 15/11/2014,2:53 PM by Charles
        private void InitialPopulation()
        {
            SafeClearTarget();
            foreach (var t in this._sourceCollection.Where(x => x != null))
            {
                _target.Add(this._projector(t));
            }
        }

        private void SafeClearTarget()
        {
            while (_target.Count > 0)
            {
                var syncitem = _target[0];
                _target.RemoveAt(0);
                if (_cleanup != null) _cleanup(syncitem);
            }
        }
    }

    /// <summary>
    /// Low cost control to display a set of clickable items
    /// </summary>
    /// <typeparam name="T">The Type of viewmodel</typeparam>
    public class RepeaterView<T> : StackLayout
        where T : class
    {
        /// <summary>
        /// Definition for <see cref="ItemTemplate"/>
        /// </summary>
        /// Element created at 15/11/2014,3:11 PM by Charles
        public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create<RepeaterView<T>, DataTemplate>(
                p => p.ItemTemplate,
                default(DataTemplate));

        /// <summary>
        /// Definition for <see cref="ItemsSource"/>
        /// </summary>
        /// Element created at 15/11/2014,3:11 PM by Charles
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create<RepeaterView<T>, IEnumerable<T>>(
                p => p.ItemsSource,
                Enumerable.Empty<T>(),
                BindingMode.OneWay,
                null,
                ItemsChanged);

        /// <summary>
        /// Definition for <see cref="ItemClickCommand"/>
        /// </summary>
        /// Element created at 15/11/2014,3:11 PM by Charles
        public static BindableProperty ItemClickCommandProperty =
            BindableProperty.Create<RepeaterView<T>, ICommand>(x => x.ItemClickCommand, null);

        /// <summary>
        /// Definition for <see cref="TemplateSelector"/>
        /// </summary>
        /// Element created at 15/11/2014,3:12 PM by Charles
        public static readonly BindableProperty TemplateSelectorProperty =
            BindableProperty.Create<RepeaterView<T>, TemplateSelector>(
                x => x.TemplateSelector,
                default(TemplateSelector));

        /// <summary>
        /// Event delegate definition fo the <see cref="ItemCreated"/> event
        /// </summary>
        /// <param name="sender">The sender(this).</param>
        /// <param name="args">The <see cref="RepeaterViewItemAddedEventArgs"/> instance containing the event data.</param>
        /// Element created at 15/11/2014,3:12 PM by Charles
        public delegate void RepeaterViewItemAddedEventHandler(
            object sender,
            RepeaterViewItemAddedEventArgs args);

        /// <summary>Occurs when a view has been created.</summary>
        /// Element created at 15/11/2014,3:13 PM by Charles
        public event RepeaterViewItemAddedEventHandler ItemCreated;

        /// <summary>
        /// The Collection changed handler
        /// </summary>
        /// Element created at 15/11/2014,3:13 PM by Charles
        private IDisposable _collectionChangedHandle;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepeaterView{T}"/> class.
        /// </summary>
        /// Element created at 15/11/2014,3:13 PM by Charles
        public RepeaterView()
        {
            Spacing = 0;
        }

        /// <summary>Gets or sets the items source.</summary>
        /// <value>The items source.</value>
        /// Element created at 15/11/2014,3:13 PM by Charles
        public IEnumerable<T> ItemsSource
        {
            get { return (IEnumerable<T>) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>Gets or sets the template selector.</summary>
        /// <value>The template selector.</value>
        /// Element created at 15/11/2014,3:13 PM by Charles
        public TemplateSelector TemplateSelector
        {
            get { return (TemplateSelector) GetValue(TemplateSelectorProperty); }
            set { SetValue(TemplateSelectorProperty, value); }
        }

        /// <summary>Gets or sets the item click command.</summary>
        /// <value>The item click command.</value>
        /// Element created at 15/11/2014,3:13 PM by Charles
        public ICommand ItemClickCommand
        {
            get { return (ICommand) this.GetValue(ItemClickCommandProperty); }
            set { SetValue(ItemClickCommandProperty, value); }
        }

        /// <summary>
        /// The item template property
        /// This can be used on it's own or in combination with 
        /// the <see cref="TemplateSelector"/>
        /// </summary>
        /// Element created at 15/11/2014,3:10 PM by Charles
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate) GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        /// Gives codebehind a chance to play with the
        /// newly created view object :D
        /// </summary>
        /// <param name="view">The visual view object</param>
        /// <param name="model">The item being added</param>
        protected virtual void NotifyItemAdded(View view, T model)
        {
            if (ItemCreated != null)
            {
                ItemCreated(this, new RepeaterViewItemAddedEventArgs(view, model));
            }
        }

        /// <summary>
        /// Select a datatemplate dynamically
        /// Prefer the TemplateSelector then the DataTemplate
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual DataTemplate GetTemplateFor(Type type)
        {
            DataTemplate retTemplate = null;
            if (TemplateSelector != null) retTemplate = TemplateSelector.TemplateFor(type);
            return retTemplate ?? ItemTemplate;
        }

        /// <summary>
        /// Creates a view based on the items type
        /// While we do have T, T could very well be
        /// a common superclass or an interface by
        /// using the items actual type we support
        /// both inheritance based polymorphism
        /// and shape based polymorphism
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns>A View that has been initialized with <see cref="item"/> as it's BindingContext</returns>
        /// <exception cref="InvalidVisualObjectException"></exception>Thrown when the matched datatemplate inflates to an object not derived from either
        /// <see cref="Xamarin.Forms.View"/> or <see cref="Xamarin.Forms.ViewCell"/>
        protected virtual View ViewFor(T item)
        {
            var template = GetTemplateFor(item.GetType());
            var content = template.CreateContent();

            if (!(content is View) && !(content is ViewCell)) throw new InvalidVisualObjectException(content.GetType());
            var view = (content is View) ? content as View : ((ViewCell) content).View;
            view.BindingContext = item;
            view.GestureRecognizers.Add(
                new TapGestureRecognizer {Command = ItemClickCommand, CommandParameter = item});
            return view;
        }

        /// <summary>
        /// Reset the collection of bound objects
        /// Remove the old collection changed eventhandler (if any)
        /// Create new cells for each new item
        /// </summary>
        /// <param name="bindable">The control</param>
        /// <param name="oldValue">Previous bound collection</param>
        /// <param name="newValue">New bound collection</param>
        private static void ItemsChanged(
            BindableObject bindable,
            IEnumerable<T> oldValue,
            IEnumerable<T> newValue)
        {
            var control = bindable as RepeaterView<T>;
            if (control == null)
                throw new Exception(
                    "Invalid bindable object passed to ReapterView::ItemsChanged expected a ReapterView<T> received a "
                    + bindable.GetType().Name);

            if (control._collectionChangedHandle != null)
            {
                control._collectionChangedHandle.Dispose();
            }

            control._collectionChangedHandle = new CollectionChangedHandle<View, T>(
                control.Children,
                newValue,
                control.ViewFor,
                (v, m, i) => control.NotifyItemAdded(v, m));
        }
    }

    /// <summary>
    /// Argument for the <see cref="RepeaterView{T}.ItemCreated"/> event
    /// </summary>
    /// Element created at 15/11/2014,3:13 PM by Charles
    public class RepeaterViewItemAddedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepeaterViewItemAddedEventArgs"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="model">The model.</param>
        /// Element created at 15/11/2014,3:14 PM by Charles
        public RepeaterViewItemAddedEventArgs(View view, object model)
        {
            View = view;
            Model = model;
        }

        /// <summary>Gets or sets the view.</summary>
        /// <value>The visual element.</value>
        /// Element created at 15/11/2014,3:14 PM by Charles
        public View View { get; set; }

        /// <summary>Gets or sets the model.</summary>
        /// <value>The original viewmodel.</value>
        /// Element created at 15/11/2014,3:14 PM by Charles
        public object Model { get; set; }
    }


    public class TemplateSelector : BindableObject
    {
        /// <summary>
        /// Property definition for the <see cref="Templates"/> Bindable Property
        /// </summary>
        public static BindableProperty TemplatesProperty =
            BindableProperty.Create<TemplateSelector, DataTemplateCollection>(x => x.Templates,
                default(DataTemplateCollection), BindingMode.OneWay, null, TemplatesChanged);

        /// <summary>
        /// Property definition for the <see cref="SelectorFunction"/> Bindable Property
        /// </summary>
        public static BindableProperty SelectorFunctionProperty =
            BindableProperty.Create<TemplateSelector, Func<Type, DataTemplate>>(x => x.SelectorFunction, null);

        /// <summary>
        /// Property definition for the <see cref="ExceptionOnNoMatch"/> Bindable Property
        /// </summary>
        public static BindableProperty ExceptionOnNoMatchProperty =
            BindableProperty.Create<TemplateSelector, bool>(x => x.ExceptionOnNoMatch, true);

        /// <summary>
        /// Initialize the TemplateCollections so that each 
        /// instance gets it's own collection
        /// </summary>
        public TemplateSelector()
        {
            Templates = new DataTemplateCollection();
        }

        /// <summary>
        ///  Clears the cache when the set of templates change
        /// </summary>
        /// <param name="bo"></param>
        /// <param name="oldval"></param>
        /// <param name="newval"></param>
        public static void TemplatesChanged(BindableObject bo, DataTemplateCollection oldval,
            DataTemplateCollection newval)
        {
            var ts = bo as TemplateSelector;
            if (ts == null) return;
            if (oldval != null) oldval.CollectionChanged -= ts.TemplateSetChanged;
            newval.CollectionChanged += ts.TemplateSetChanged;
            ts.Cache = null;
        }

        /// <summary>
        /// Clear the cache on any template set change
        /// If needed this could be optimized to care about the specific
        /// change but I doubt it would be worthwhile.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TemplateSetChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Cache = null;
        }

        /// <summary>
        /// Private cache of matched types with datatemplates
        /// The cache is reset on any change to <see cref="Templates"/>
        /// </summary>
        private Dictionary<Type, DataTemplate> Cache { get; set; }

        /// <summary>
        /// Bindable property that allows the user to 
        /// determine if a <see cref="NoDataTemplateMatchException"/> is thrown when 
        /// there is no matching template found
        /// </summary>
        public bool ExceptionOnNoMatch
        {
            get { return (bool) GetValue(ExceptionOnNoMatchProperty); }
            set { SetValue(ExceptionOnNoMatchProperty, value); }
        }

        /// <summary>
        /// The collection of DataTemplates
        /// </summary>
        public DataTemplateCollection Templates
        {
            get { return (DataTemplateCollection) GetValue(TemplatesProperty); }
            set { SetValue(TemplatesProperty, value); }
        }

        /// <summary>
        /// A user supplied function of type
        /// <code>Func<typeparamname name="Type"></typeparamname>,<typeparamname name="DataTemplate"></typeparamname></code>
        /// If this function has been supplied it is always called first in the match 
        /// process.
        /// </summary>
        public Func<Type, DataTemplate> SelectorFunction
        {
            get { return (Func<Type, DataTemplate>) GetValue(SelectorFunctionProperty); }
            set { SetValue(SelectorFunctionProperty, value); }
        }


        /// <summary>
        /// Matches a type with a datatemplate
        /// Order of matching=>
        ///     SelectorFunction, 
        ///     Cache, 
        ///     SpecificTypeMatch,
        ///     InterfaceMatch,
        ///     BaseTypeMatch 
        ///     DefaultTempalte
        /// </summary>
        /// <param name="type">Type object type that needs a datatemplate</param>
        /// <returns>The DataTemplate from the WrappedDataTemplates Collection that closest matches 
        /// the type paramater.</returns>
        /// <exception cref="NoDataTemplateMatchException"></exception>Thrown if there is no datatemplate that matches the supplied type
        public DataTemplate TemplateFor(Type type)
        {
            var typesExamined = new List<Type>();
            var template = TemplateForImpl(type, typesExamined);
            if (template == null && ExceptionOnNoMatch)
                throw new NoDataTemplateMatchException(type, typesExamined);
            return template;
        }

        /// <summary>
        /// Interal implementation of <see cref="TemplateFor"/>.
        /// </summary>
        /// <param name="type">The type to match on</param>
        /// <param name="examined">A list of all types examined during the matching process</param>
        /// <returns>A DataTemplate or null</returns>
        private DataTemplate TemplateForImpl(Type type, List<Type> examined)
        {
            if (type == null) return null; //This can happen when we recusively check base types (object.BaseType==null)
            examined.Add(type);
            Contract.Assert(Templates != null, "Templates cannot be null");

            Cache = Cache ?? new Dictionary<Type, DataTemplate>();
            DataTemplate retTemplate = null;

            //Prefer the selector function if present
            //This has been moved before the cache check so that
            //the user supplied function has an opportunity to 
            //Make a decision with more information than simply
            //the requested type (perhaps the Ux or Network states...)
            if (SelectorFunction != null) retTemplate = SelectorFunction(type);

            //Happy case we already have the type in our cache
            if (Cache.ContainsKey(type)) return Cache[type];


            //check our list
            retTemplate = Templates.Where(x => x.Type == type).Select(x => x.WrappedTemplate).FirstOrDefault();
            //Check for interfaces
            retTemplate = retTemplate ??
                          type.GetTypeInfo()
                              .ImplementedInterfaces.Select(x => TemplateForImpl(x, examined))
                              .FirstOrDefault();
            //look at base types
            retTemplate = retTemplate ?? TemplateForImpl(type.GetTypeInfo().BaseType, examined);
            //If all else fails try to find a Default Template
            retTemplate = retTemplate ??
                          Templates.Where(x => x.IsDefault).Select(x => x.WrappedTemplate).FirstOrDefault();

            Cache[type] = retTemplate;
            return retTemplate;
        }

        /// <summary>
        /// Finds a template for the type of the passed in item (<code>item.GetType()</code>)
        /// and creates the content and sets the Binding context of the View
        /// Currently the root of the DataTemplate must be a ViewCell.
        /// </summary>
        /// <param name="item">The item to instantiate a DataTemplate for</param>
        /// <returns>a View with it's binding context set</returns>
        /// <exception cref="InvalidVisualObjectException"></exception>Thrown when the matched datatemplate inflates to an object not derived from either 
        /// <see cref="Xamarin.Forms.View"/> or <see cref="Xamarin.Forms.ViewCell"/>
        public View ViewFor(object item)
        {
            var template = TemplateFor(item.GetType());
            var content = template.CreateContent();
            if (!(content is View) && !(content is ViewCell))
                throw new InvalidVisualObjectException(content.GetType());

            var view = (content is View) ? content as View : ((ViewCell) content).View;
            view.BindingContext = item;
            return view;
        }
    }

    /// <summary>
    /// Interface to enable DataTemplateCollection to hold
    /// typesafe instances of DataTemplateWrapper
    /// </summary>
    public interface IDataTemplateWrapper
    {
        bool IsDefault { get; set; }
        DataTemplate WrappedTemplate { get; set; }
        Type Type { get; }
    }

    /// <summary>
    /// Wrapper for a DataTemplate.
    /// Unfortunately the default constructor for DataTemplate is internal
    /// so I had to wrap the DataTemplate instead of inheriting it.
    /// </summary>
    /// <typeparam name="T">The object type that this DataTemplateWrapper matches</typeparam>
    public class DataTemplateWrapper<T> : BindableObject, IDataTemplateWrapper
    {
        public static readonly BindableProperty WrappedTemplateProperty =
            BindableProperty.Create<DataTemplateWrapper<T>, DataTemplate>(x => x.WrappedTemplate, null);

        public static readonly BindableProperty IsDefaultProperty =
            BindableProperty.Create<DataTemplateWrapper<T>, bool>(x => x.IsDefault, false);

        public bool IsDefault
        {
            get { return (bool) GetValue(IsDefaultProperty); }
            set { SetValue(IsDefaultProperty, value); }
        }

        public DataTemplate WrappedTemplate
        {
            get { return (DataTemplate) GetValue(WrappedTemplateProperty); }
            set { SetValue(WrappedTemplateProperty, value); }
        }

        public Type Type
        {
            get { return typeof (T); }
        }
    }

    /// <summary>
    /// Collection class of IDataTemplateWrapper
    /// Enables xaml definitions of collections.
    /// </summary>
    public class DataTemplateCollection : ObservableCollection<IDataTemplateWrapper>
    {
    }
}
