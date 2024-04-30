resource "azurerm_resource_group" "metadata_app_rg" {
  name     = var.resources_common_name
  location = "eastus"
}

resource "azurerm_app_configuration" "metadata_app_config" {
  name                = var.resources_common_name
  resource_group_name = azurerm_resource_group.metadata_app_rg.name
  location            = azurerm_resource_group.metadata_app_rg.location
}
