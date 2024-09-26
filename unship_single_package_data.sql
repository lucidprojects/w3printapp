/*
Warning! Mandatory fields and its order:
1: package_id
*/

SELECT
  packages.package_id AS `Package Id`
  , orders.InvoiceNumber AS `Invoice Number`
  , shipping_method AS `Shipping Method`
  , package_invoice_templates.template_name AS `Template Name`
  , ProductElements.element_name AS `Package Name`
  , packages.total_weight AS `Total Weight`
  , packages.shipping_cost AS `Shipping Cost`
  , packages.created AS `Created`
  , packages.shipping_date AS `Shipping Date`
  
FROM
  packages 
    INNER JOIN orders 
      ON packages.primary_order_id = orders.id 
    INNER JOIN package_invoice_templates 
      ON packages.template_id = package_invoice_templates.template_id 
    INNER JOIN ProductElements 
      ON packages.package_element_id = ProductElements.id
      
WHERE 
  packages.tracking_number = "%s"
  AND packages.status_cd IN ('PRINTABLE', 'PRINTED', 'REPRINTED')