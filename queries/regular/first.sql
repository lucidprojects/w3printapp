/*
Warning! Mandatory fields and its order:
1: package_id
2: invoice_number
3: tracking_number
*/

SELECT
  packages.package_id AS `Package Id`
  ,orders.InvoiceNumber AS `Invoice Number`
  ,packages.tracking_number AS `Tracking Number`
  ,shipping_method AS `Shipping Method`
  ,package_invoice_templates.template_name AS `Template Name`
  ,ProductElements.element_name AS `Package Name`
  ,packages.total_weight AS `Total Weight`
  ,packages.shipping_cost AS `Shipping Cost`
  ,packages.created AS `Created`
  
FROM
  packages 
    INNER JOIN orders 
      ON packages.primary_order_id = orders.id 
    INNER JOIN package_invoice_templates 
      ON packages.template_id = package_invoice_templates.template_id 
    INNER JOIN ProductElements 
      ON packages.package_element_id = ProductElements.id
  
WHERE
  packages.status_cd = 'PRINTABLE'
  AND IFNULL(packages.pmod_cd, 0) = 0
  AND UPPER(packages.shipping_method) = 'FIRST'