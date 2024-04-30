resource "azurerm_resource_group" "metadata_app_rg" {
  name     = var.resources_common_name
  location = "eastus"
}

resource "azurerm_app_configuration" "metadata_app_config" {
  name                = var.resources_common_name
  resource_group_name = azurerm_resource_group.metadata_app_rg.name
  location            = azurerm_resource_group.metadata_app_rg.location
}

resource "azurerm_app_configuration_key" "metadata_app_return_properties" {
  configuration_store_id = azurerm_app_configuration.metadata_app_config.id
  key                    = "MetadataApp:Settings:ReturnProperties"
  value                  = join(";", var.return_properties)
}

resource "random_integer" "sentinel_value" {
  min = 1
  max = 50000
  keepers = {
    # Generate a new integer each time the return properties config value is updated
    listener_arn = azurerm_app_configuration_key.metadata_app_return_properties
  }
}

resource "azurerm_app_configuration_key" "metadata_app_sentinel_key" {
  configuration_store_id = azurerm_app_configuration.metadata_app_config.id
  key                    = "MetadataApp:Settings:Sentinel"
  value                  = random_integer.sentinel_value
}
