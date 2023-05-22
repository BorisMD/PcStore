using Microsoft.Extensions.DependencyInjection;
using PcStore;
using PcStore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.DirectoryServices;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly PCBuilderContext _dbContext;
    private readonly PcStoreSearch _pcStoreSearch;
    private Type _selectedComponentType;
    public ObservableCollection<ComponentBase> results = new ObservableCollection<ComponentBase>();
    private ObservableCollection<KeyValuePair<string, string>> _filters;
    private List<Type> _componentTypes;
    private string _currentFilter;
    private string _selectedFilterOption;
    public ICommand SearchCommand { get; }
    public ICommand ClearFiltersCommand { get; }
    public ICommand AddFilterCommand { get; }
    public ObservableCollection<string> FilterOptions { get; } = new ObservableCollection<string>();


    public MainViewModel()
    {
        _dbContext = ((App)Application.Current).ServiceProvider.GetRequiredService<PCBuilderContext>();
        ComponentTypes = new List<Type> 
        {
            typeof(CPU),
            typeof(GPU),
            typeof(Motherboard),
            typeof(RAM),
            typeof(Memory),
            typeof(PowerSupply),
        };
        _pcStoreSearch = new PcStoreSearch(new PCBuilderContext());
        _filters = new ObservableCollection<KeyValuePair<string, string>>();
        SearchCommand = new RelayCommand(_ => Search(), _ => true);
        ClearFiltersCommand = new RelayCommand(_ => ClearFilters(), _ => true);
        AddFilterCommand = new RelayCommand(_ => AddFilter(), _ => true);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public List<Type> ComponentTypes
    {
        get => _componentTypes;
        set
        {
            _componentTypes = value;
            OnPropertyChanged(nameof(ComponentTypes));
        }
    }

    public ObservableCollection<ComponentBase> Results
    {
        get => results;
        set
        {
            results = value;
            OnPropertyChanged();
        }
    }

    public Type SelectedComponentType
    {
        get => _selectedComponentType;
        set
        {
            _selectedComponentType = value;

            OnPropertyChanged();
            UpdateFilterOptions();

            // Clear existing columns of the DataGrid
            var dataGrid = Application.Current.MainWindow?.FindName("resultsDataGrid") as DataGrid;
            dataGrid?.Columns.Clear();

            // Clear existing results
            Results.Clear();

            // Get properties of the selected component type
            var properties = _selectedComponentType.GetProperties();

            // Add common properties to the DataGrid
            foreach (var property in properties)
            {
                dataGrid?.Columns.Add(new DataGridTextColumn()
                {
                    Header = property.Name,
                    Binding = new Binding(property.Name)
                });
            }

            // Get new results
            var components = _pcStoreSearch.GetAllComponents(_selectedComponentType);
            foreach (var component in components)
            {
                Results.Add(component);
            }
        }
    }

    public ObservableCollection<KeyValuePair<string, string>> Filters
    {
        get { return _filters; }
        set
        {
            _filters = value;
            OnPropertyChanged();
        }
    }

    public string CurrentFilter
    {
        get => _currentFilter;
        set
        {
            _currentFilter = value;
            OnPropertyChanged();
        }
    }

    public string SelectedFilterOption
    {
        get => _selectedFilterOption;
        set
        {
            _selectedFilterOption = value;
            OnPropertyChanged();
        }
    }

    private void UpdateFilterOptions()
    {
        FilterOptions.Clear();
        if (_selectedComponentType != null)
        {
            foreach (var property in _selectedComponentType.GetProperties())
            {
                FilterOptions.Add(property.Name);
            }
        }
    }

    private void Search()
    {
        // Clear existing results
        Results.Clear();

        // Check if a component type has been selected
        if (_selectedComponentType != null)
        {
            // Get the search method with generic parameter
            var searchMethod = typeof(PcStoreSearch)
                .GetMethod("Search", new Type[] { typeof(ObservableCollection<KeyValuePair<string, string>>) })
                .MakeGenericMethod(_selectedComponentType);

            // Invoke the search method with filters as parameters
            var components = searchMethod.Invoke(_pcStoreSearch, new object[] { _filters }) as IEnumerable<ComponentBase>;

            // Check if the search returned any results
            if (components != null)
            {
                // Add the returned components to the Results
                foreach (var component in components)
                {
                    Results.Add(component);
                }
            }
        }
    }

    private void AddFilter()
    {
        if (!string.IsNullOrEmpty(CurrentFilter) && !string.IsNullOrEmpty(SelectedFilterOption))
        {
            Filters.Add(new KeyValuePair<string, string>(SelectedFilterOption, CurrentFilter));
        }
    }

    private void ClearFilters()
    {
        Filters.Clear();
    }

    private void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}