/*
Warning! Mandatory fields and its order:
1: tracking_number
2: package_id
*/

SELECT
  packages.tracking_number AS `Tracking Number`
  ,packages.package_id AS `Package Id`
  ,orders.InvoiceNumber AS `Invoice Number`
  ,shipping_method AS `Shipping Method`
  ,package_invoice_templates.template_name AS `Template Name`
  ,ProductElements.element_name AS `Package Name`
  ,packages.total_weight AS `Total Weight`
  ,packages.shipping_cost AS `Shipping Cost`
  ,packages.created AS `Created`
  ,packages.shipping_date AS `Shipping Date`
  
FROM
  package_print_batches 
    INNER JOIN packages 
      ON package_print_batches.package_id = packages.package_id 
    INNER JOIN orders 
      ON packages.primary_order_id = orders.id 
    INNER JOIN package_invoice_templates 
      ON packages.template_id = package_invoice_templates.template_id 
    INNER JOIN ProductElements 
      ON packages.package_element_id = ProductElements.id

WHERE
  packages.status_cd IN ('PRINTED', 'REPRINTED')
  AND package_print_batches.print_batch_id = %d  