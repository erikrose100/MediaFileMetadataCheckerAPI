variable "resources_common_name" {
  type        = string
  description = "Name shared between resources"
}

variable "return_properties" {
  type        = list(string)
  description = "List of strings containing properties of media files for app to return"
}
