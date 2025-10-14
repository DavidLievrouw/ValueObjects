namespace Dalion.ValueObjects.Generation.Fragments;

internal interface IFragmentProvider
{
    string ProvideFragment(AttributeConfiguration config);
}