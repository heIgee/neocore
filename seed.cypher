create (i1:Item {id: 1, name: 'Intel i7 Processor', type: 'Component', manufacturer: 'Intel', specifications: '3.6GHz, 8 cores', price: 300.0})
create (i2:Item {id: 2, name: 'NVIDIA GeForce RTX 3080', type: 'Component', manufacturer: 'NVIDIA', specifications: '10GB GDDR6X', price: 700.0})
create (i3:Item {id: 3, name: '1TB SSD', type: 'Component', manufacturer: 'Samsung', specifications: 'NVMe, 3500MB/s read', price: 150.0})
create (i4:Item {id: 4, name: '27" 4K Monitor', type: 'Peripheral', manufacturer: 'LG', specifications: '3840x2160, 60Hz', price: 400.0})
create (i5:Item {id: 5, name: 'Mechanical Keyboard', type: 'Peripheral', manufacturer: 'Corsair', specifications: 'Cherry MX switches', price: 120.0})
create (i6:Item {id: 6, name: 'Gaming Laptop', type: 'Laptop', manufacturer: 'ASUS', specifications: 'i7, 16GB RAM, RTX 3070', price: 1500.0})
create (i7:Item {id: 7, name: 'AMD Ryzen 9 Processor', type: 'Component', manufacturer: 'AMD', specifications: '3.8GHz, 12 cores', price: 400.0})
create (i8:Item {id: 8, name: 'NVIDIA GeForce RTX 3070', type: 'Component', manufacturer: 'NVIDIA', specifications: '8GB GDDR6', price: 500.0})
create (i9:Item {id: 9, name: '2TB SSD', type: 'Component', manufacturer: 'Samsung', specifications: 'NVMe, 3500MB/s read', price: 250.0})
create (i10:Item {id: 10, name: '24" 1080p Monitor', type: 'Peripheral', manufacturer: 'LG', specifications: '1920x1080, 75Hz', price: 200.0})
create (i11:Item {id: 11, name: 'Wireless Mouse', type: 'Peripheral', manufacturer: 'Logitech', specifications: '2.4GHz, 16000 DPI', price: 70.0})
create (i12:Item {id: 12, name: 'Ultrabook', type: 'Laptop', manufacturer: 'Dell', specifications: 'i5, 8GB RAM, Intel Iris Xe', price: 1000.0})

create (s1:Vendor {id: 1, name: 'TechSupply Co.', contactInfo: 'techsupply@example.com'})
create (s2:Vendor {id: 2, name: 'ComponentMaster', contactInfo: 'componentmaster@example.com'})
create (s3:Vendor {id: 3, name: 'GadgetWorld', contactInfo: 'gadgetworld@example.com'})
create (s4:Vendor {id: 4, name: 'CompuHub', contactInfo: 'compuhub@example.com'})
create (s5:Vendor {id: 5, name: 'PeripheralPro', contactInfo: 'peripheralpro@example.com'})
create (s6:Vendor {id: 6, name: 'LaptopLand', contactInfo: 'laptopland@example.com'})

create (c1:Contract {id: 1, deliveryDate: date('2024-07-01')})
create (c2:Contract {id: 2, deliveryDate: date('2024-07-05')})
create (c3:Contract {id: 3, deliveryDate: date('2024-07-10')})
create (c4:Contract {id: 4, deliveryDate: date('2024-07-15')})
create (c5:Contract {id: 5, deliveryDate: date('2024-07-20')})
create (c6:Contract {id: 6, deliveryDate: date('2024-07-25')})
create (c7:Contract {id: 7, deliveryDate: date('2024-07-30')})
create (c8:Contract {id: 8, deliveryDate: date('2024-08-01')})

create (u1:User {id: 1, name: 'Don', password: 'owner', role: 'owner'})
create (u2:User {id: 2, name: 'Joe', password: 'editor', role: 'editor'})
create (u3:User {id: 3, name: 'Kam', password: 'viewer', role: 'viewer'})

// Customers
create (cu1:Customer {id: 1, fullName: 'John Smith'})
create (cu2:Customer {id: 2, fullName: 'Mary Johnson'})
create (cu3:Customer {id: 3, fullName: 'Robert Davis'})

// Employees
create (e1:Employee {id: 1, fullName: 'Alice Wilson'})
create (e2:Employee {id: 2, fullName: 'Bob Miller'})
create (e3:Employee {id: 3, fullName: 'Carol Brown'})

// Sales
create (sa1:Sale {id: 1, total: 1500.0, date: date('2024-10-01')})
create (sa2:Sale {id: 2, total: 2200.0, date: date('2024-10-05')})
create (sa3:Sale {id: 3, total: 800.0, date: date('2024-10-08')})

// Repairs
create (r1:Repair {id: 1, status: 'HandedOver', isWarranty: true, cause: 'Faulty RAM', price: 0.0, handedDate: date('2024-10-02'), returnedDate: null})
create (r2:Repair {id: 2, status: 'Returned', isWarranty: false, cause: 'Broken screen', price: 200.0, handedDate: date('2024-09-25'), returnedDate: date('2024-10-01')})
create (r3:Repair {id: 3, status: 'InProgress', isWarranty: true, cause: 'GPU artifacts', price: 0.0, handedDate: date('2024-10-07'), returnedDate: null})

// Builds
create (b1:Build {id: 1, name: 'Gaming PC', price: 2000.0})
create (b2:Build {id: 2, name: 'Office PC', price: 1200.0})

///

create (c1)-[:SIGNED_WITH]->(s1)
create (c2)-[:SIGNED_WITH]->(s1)
create (c3)-[:SIGNED_WITH]->(s2)
create (c4)-[:SIGNED_WITH]->(s2)
create (c5)-[:SIGNED_WITH]->(s3)
create (c6)-[:SIGNED_WITH]->(s3)
create (c7)-[:SIGNED_WITH]->(s4)
create (c8)-[:SIGNED_WITH]->(s5)

create (i1)-[:SUPPLIED_UNDER {quantity: 50}]->(c1)
create (i1)-[:SUPPLIED_UNDER {quantity: 25}]->(c2)
create (i2)-[:SUPPLIED_UNDER {quantity: 30}]->(c1)
create (i3)-[:SUPPLIED_UNDER {quantity: 100}]->(c3)
create (i3)-[:SUPPLIED_UNDER {quantity: 50}]->(c4)
create (i4)-[:SUPPLIED_UNDER {quantity: 40}]->(c3)
create (i5)-[:SUPPLIED_UNDER {quantity: 60}]->(c5)
create (i6)-[:SUPPLIED_UNDER {quantity: 20}]->(c5)

create (i7)-[:SUPPLIED_UNDER {quantity: 70}]->(c6)
create (i7)-[:SUPPLIED_UNDER {quantity: 35}]->(c7)
create (i8)-[:SUPPLIED_UNDER {quantity: 50}]->(c6)
create (i9)-[:SUPPLIED_UNDER {quantity: 120}]->(c4)
create (i9)-[:SUPPLIED_UNDER {quantity: 60}]->(c8)
create (i10)-[:SUPPLIED_UNDER {quantity: 80}]->(c4)
create (i11)-[:SUPPLIED_UNDER {quantity: 90}]->(c7)
create (i12)-[:SUPPLIED_UNDER {quantity: 30}]->(c8)

// Sale relationships
create (sa1)-[:ORDERED_BY]->(cu1)
create (sa2)-[:ORDERED_BY]->(cu2)
create (sa3)-[:ORDERED_BY]->(cu3)

create (sa1)-[:SOLD_BY]->(e1)
create (sa2)-[:SOLD_BY]->(e2)
create (sa3)-[:SOLD_BY]->(e3)

create (sa1)-[:INCLUDES {quantity: 1, warrantyTerms: '2 years standard warranty'}]->(i6)
create (sa2)-[:INCLUDES {quantity: 1, warrantyTerms: '3 years extended warranty'}]->(i1)
create (sa2)-[:INCLUDES {quantity: 1, warrantyTerms: '3 years extended warranty'}]->(i2)
create (sa3)-[:INCLUDES {quantity: 2, warrantyTerms: '1 year standard warranty'}]->(i4)

// Repair relationships
create (r1)-[:INVOLVES]->(i1)
create (r2)-[:INVOLVES]->(i4)
create (r3)-[:INVOLVES]->(i2)

create (r1)-[:REQUESTED_BY]->(cu2)
create (r2)-[:REQUESTED_BY]->(cu3)
create (r3)-[:REQUESTED_BY]->(cu2)

create (r1)-[:HANDLED_BY]->(e2)
create (r2)-[:HANDLED_BY]->(e1)
create (r3)-[:HANDLED_BY]->(e3)

// Build relationships
create (b1)-[:CONSISTS_OF]->(i1)
create (b1)-[:CONSISTS_OF]->(i2)
create (b1)-[:CONSISTS_OF]->(i3)
create (b1)-[:CONSISTS_OF]->(i5)

create (b2)-[:CONSISTS_OF]->(i7)
create (b2)-[:CONSISTS_OF]->(i9)
create (b2)-[:CONSISTS_OF]->(i10)
create (b2)-[:CONSISTS_OF]->(i11)

create (b1)-[:ORDERED_BY]->(cu1)
create (b2)-[:ORDERED_BY]->(cu3)

create (b1)-[:SOLD_BY]->(e1)
create (b2)-[:SOLD_BY]->(e2)