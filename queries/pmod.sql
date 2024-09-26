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
  , GROUP_CONCAT(CONCAT(FulfilableElements.element_num,"-",orderLineItems.Quantity * items.product_quantity * Products_to_ProductElements.element_qty)ORDER BY FulfilableElements.element_num) AS `Elements Ordered`
FROM
  packages
    INNER JOIN orders
      ON packages.primary_order_id = orders.id
    INNER JOIN package_invoice_templates
      ON packages.template_id = package_invoice_templates.template_id
    INNER JOIN ProductElements
      ON packages.package_element_id = ProductElements.id
    LEFT OUTER JOIN orderLineItems
      ON orders.InvoiceNumber = orderLineItems.InvoiceNumber
      AND orders.TransactionID = orderLineItems.TransactionID
    LEFT OUTER JOIN items
      ON orderLineItems.ItemNumber = items.ItemNumber
    LEFT OUTER JOIN Products_to_ProductElements
      ON items.products_id = Products_to_ProductElements.product_id
    LEFT OUTER JOIN ProductElements AS FulfilableElements
      ON Products_to_ProductElements.element_id = FulfilableElements.id
      AND FulfilableElements.element_type = "PRODUCT"
WHERE
  packages.status_cd = 'PRINTABLE'
  AND packages.pmod_cd = 1
GROUP BY
  packages.package_id
ORDER BY  
  `Template Name`
  ,`Shipping Method`
  ,`Elements Ordered`