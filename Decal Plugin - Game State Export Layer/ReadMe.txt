Notes: Decal can handle finding a vendor and opening trade with them via the Utility Belt plugin they have a subplugin named Autotrader that contains the logic for this. 


https://gitlab.com/utilitybelt/utilitybelt.gitlab.io

Example

Let's say we have a vendor object:

WorldObject vendor = WorldFilter[123456789];

Then:

Actions.UseItem(vendor.Id, 0);

is equivalent to the player clicking the vendor.

After the server accepts it, Decal fires:

WorldFilter.ApproachVendor

Example:

WorldFilter.ApproachVendor += VendorOpened;

private void VendorOpened(object sender, ApproachVendorEventArgs e)
{
    Console.WriteLine($"Vendor opened: {e.MerchantId}");
}

Now:

WorldFilter.OpenVendor

will return the active vendor.