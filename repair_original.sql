/*
Warning! Mandatory fields and its order:
1: package_id
*/

SELECT
  packages.package_id AS `Package Id`
  ,packages.status_cd AS `Status`
  ,orders.InvoiceNumber AS `Invoice Number`
  ,packages.shipping_method AS `Shipping Method`
  ,package_invoice_templates.template_name AS `Template Name`
  ,packages.tracking_number AS `Tracking Number`
  ,ProductElements.element_name AS `Package Name`
  ,packages.total_weight AS `Total Weight`
  ,packages.shipping_cost AS `Shipping Cost`
  ,packages.created AS `Created`
  ,packages.pmod_cd AS `Is PMOD`
FROM
  packages 
    LEFT JOIN orders 
      ON packages.primary_order_id = orders.id 
    LEFT JOIN package_invoice_templates 
      ON packages.template_id = package_invoice_templates.template_id 
    LEFT JOIN ProductElements 
      ON packages.package_element_id = ProductElements.id
  
WHERE
  (packages.status_cd IN ('LABELABLE_LOCKED', 'LABELED', 'PRINTABLE_LOCKED', 'PRINTED_LOCKED', 'REPRINTED_LOCKED')
    AND TIME_TO_SEC(TIMEDIFF(NOW(), packages.edited)) / 60 > 10)
  OR packages.status_cd = 'ERROR'
