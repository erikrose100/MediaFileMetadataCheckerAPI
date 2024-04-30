module "metdata_app_config" {
  source                = "./modules/metadata_app_config"
  resources_common_name = var.resources_common_name
  return_properties     = var.return_properties
}
